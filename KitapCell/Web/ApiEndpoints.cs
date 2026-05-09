using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using KitapCell.Data;
using KitapCell.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KitapCell.Web
{
    public static class ApiEndpoints
    {
        private const string SESSION_COOKIE = "kitapcell_session";

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // ── Yardımcılar ─────────────────────────────────────────────────────────

        static int? GetUserId(HttpContext ctx)
            => SessionHelper.GetUserId(ctx.Request.Cookies[SESSION_COOKIE]);

        static IResult Unauthorized()
            => Results.Json(new { error = "Giriş yapmanız gerekiyor." }, statusCode: 401);

        /// <summary>
        /// Misafir modunda oturum açmadan erişime izin verir.
        /// Login gerekli modunda oturum yoksa Unauthorized döner.
        /// Her iki durumda da null dönmesi "devam et" anlamına gelir.
        /// </summary>
        static IResult? RequireAuthOrGuest(HttpContext ctx)
        {
            if (GetUserId(ctx) is not null) return null;           // oturum var → geç
            if (!KitapCell.Core.SettingsManager.Config.RequireLoginForWebServer) return null; // misafir modu → geç
            return Unauthorized();                                  // login gerekli → 401
        }

        static async Task<T?> ReadJson<T>(HttpContext ctx)
        {
            try
            {
                using var reader = new StreamReader(ctx.Request.Body);
                var body = await reader.ReadToEndAsync();
                return JsonSerializer.Deserialize<T>(body, JsonOpts);
            }
            catch { return default; }
        }

        // ── Route Kaydı ─────────────────────────────────────────────────────────

        public static void Map(WebApplication app)
        {
            MapAuth(app);
            MapBooks(app);
            MapCategories(app);
            MapFiles(app);
            MapSocial(app);
            MapAdmin(app);
            MapExtras(app);

            // SPA fallback — eşleşmeyen route'lar index.html'e gider
            app.MapFallbackToFile("index.html");
        }

        // ── AUTH ─────────────────────────────────────────────────────────────────

        static void MapAuth(WebApplication app)
        {
            // POST /api/auth/login
            app.MapPost("/api/auth/login", async (HttpContext ctx) =>
            {
                var data = await ReadJson<LoginDto>(ctx);
                if (data is null) return Results.BadRequest(new { error = "Geçersiz istek." });

                // ── Rate limiting check ────────────────────────────────────
                var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                if (SessionHelper.IsRateLimited(ip))
                {
                    int mins = SessionHelper.LockRemainingMinutes(ip);
                    return Results.Json(
                        new { error = $"Fazla başarısız deneme. Lütfen {mins} dakika bekleyin." },
                        statusCode: 429);
                }

                // ── User verification ───────────────────────────────────────
                using var db = new LibraryDbContext();
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == data.Email);

                // Verify password using BCrypt
                bool passwordOk = user is not null && Services.PasswordHelper.Verify(data.Password, user.PasswordHash);

                if (user is null || !passwordOk)
                {
                    SessionHelper.RecordFailedAttempt(ip);
                    WebServer.AddLog($"Başarısız giriş: {data.Email} ({ip})");
                    return Results.Json(new { error = "Email veya şifre hatalı." }, statusCode: 401);
                }

                if (!user.IsActive)
                    return Results.Json(new { error = "Hesabınız pasif. Yöneticiye başvurun." }, statusCode: 403);

                // Successful login → reset the rate-limit counter
                SessionHelper.ClearFailedAttempts(ip);

                var token = SessionHelper.CreateSession(user.Id);
                ctx.Response.Cookies.Append(SESSION_COOKIE, token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                WebServer.AddLog($"Giriş: {user.Email} ({ip})");
                return Results.Ok(MapUserDto(user));
            });

            // POST /api/auth/register
            app.MapPost("/api/auth/register", async (HttpContext ctx) =>
            {
                var data = await ReadJson<RegisterDto>(ctx);
                if (data is null) return Results.BadRequest(new { error = "Geçersiz istek." });

                if (string.IsNullOrWhiteSpace(data.FirstName) || string.IsNullOrWhiteSpace(data.LastName) ||
                    string.IsNullOrWhiteSpace(data.Email)     || string.IsNullOrWhiteSpace(data.Password))
                    return Results.Json(new { error = "Tüm alanlar zorunludur." }, statusCode: 400);

                if (data.Password.Length < 6)
                    return Results.Json(new { error = "Şifre en az 6 karakter olmalıdır." }, statusCode: 400);

                using var db = new LibraryDbContext();
                if (await db.Users.AnyAsync(u => u.Email == data.Email))
                    return Results.Json(new { error = "Bu e-posta adresi zaten kayıtlı." }, statusCode: 400);

                var user = new User
                {
                    FirstName        = data.FirstName.Trim(),
                    LastName         = data.LastName.Trim(),
                    Email            = data.Email.Trim().ToLowerInvariant(),
                    PasswordHash     = Services.PasswordHelper.Hash(data.Password),
                    IdentityNumber   = string.Empty,
                    Role             = UserRole.Uye,
                    RegistrationDate = DateTime.Now,
                    IsActive         = true
                };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var token = SessionHelper.CreateSession(user.Id);
                ctx.Response.Cookies.Append(SESSION_COOKIE, token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                WebServer.AddLog($"Yeni kayıt: {user.Email}");
                return Results.Ok(MapUserDto(user));
            });

            // POST /api/auth/logout
            app.MapPost("/api/auth/logout", (HttpContext ctx) =>
            {
                SessionHelper.RemoveSession(ctx.Request.Cookies[SESSION_COOKIE]);
                ctx.Response.Cookies.Delete(SESSION_COOKIE);
                return Results.Ok(new { success = true });
            });

            // GET /api/auth/me
            app.MapGet("/api/auth/me", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                using var db = new LibraryDbContext();
                var user = await db.Users.FindAsync(uid);
                return user is null ? Results.NotFound() : Results.Ok(MapUserDto(user));
            });

            // PUT /api/me/profile
            app.MapPut("/api/me/profile", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                var data = await ReadJson<ProfileUpdateDto>(ctx);
                if (data is null || string.IsNullOrWhiteSpace(data.FirstName) || string.IsNullOrWhiteSpace(data.LastName))
                    return Results.BadRequest(new { error = "Ad ve Soyad alanları zorunludur." });

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(uid);
                if (u is null) return Results.NotFound();

                u.FirstName = data.FirstName.Trim();
                u.LastName = data.LastName.Trim();
                u.Phone = data.Phone?.Trim();
                await db.SaveChangesAsync();

                return Results.Ok(MapUserDto(u));
            });

            // PUT /api/me/password
            app.MapPut("/api/me/password", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                var data = await ReadJson<PasswordUpdateDto>(ctx);
                if (data is null || string.IsNullOrWhiteSpace(data.CurrentPassword) || string.IsNullOrWhiteSpace(data.NewPassword))
                    return Results.BadRequest(new { error = "Tüm alanları doldurun." });

                if (data.NewPassword.Length < 6)
                    return Results.BadRequest(new { error = "Yeni şifre en az 6 karakter olmalıdır." });

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(uid);
                if (u is null) return Results.NotFound();

                if (!Services.PasswordHelper.Verify(data.CurrentPassword, u.PasswordHash))
                    return Results.BadRequest(new { error = "Mevcut şifreniz yanlış." });

                u.PasswordHash = Services.PasswordHelper.Hash(data.NewPassword);
                await db.SaveChangesAsync();

                return Results.Ok(new { success = true });
            });

            // POST /api/me/avatar
            app.MapPost("/api/me/avatar", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                if (!ctx.Request.HasFormContentType)
                    return Results.BadRequest(new { error = "Geçersiz içerik tipi." });

                var form = await ctx.Request.ReadFormAsync();
                var file = form.Files.GetFile("avatar");
                if (file is null || file.Length == 0)
                    return Results.BadRequest(new { error = "Dosya seçilmedi." });

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".bmp" && ext != ".webp")
                    return Results.BadRequest(new { error = "Sadece .jpg, .png, .bmp veya .webp yükleyebilirsiniz." });

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(uid);
                if (u is null) return Results.NotFound();

                var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "profiles");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var newFileName = $"profile_{u.Id}_{Guid.NewGuid()}{ext}";
                var newPath = Path.Combine(uploadsFolder, newFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(u.ProfileImagePath) && File.Exists(u.ProfileImagePath))
                {
                    try { File.Delete(u.ProfileImagePath); } catch { }
                }

                u.ProfileImagePath = newPath;
                await db.SaveChangesAsync();

                return Results.Ok(MapUserDto(u));
            });

            // GET /api/me/avatar
            app.MapGet("/api/me/avatar", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Results.Redirect("/favicon.ico");

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(uid);
                if (u is null || string.IsNullOrEmpty(u.ProfileImagePath) || !File.Exists(u.ProfileImagePath))
                    return Results.Redirect("/favicon.ico");

                var ext = Path.GetExtension(u.ProfileImagePath).ToLowerInvariant();
                var mime = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".webp" => "image/webp",
                    ".bmp" => "image/bmp",
                    _ => "image/jpeg"
                };

                return Results.File(u.ProfileImagePath, mime);
            });
        }

        // ── BOOKS ─────────────────────────────────────────────────────────────────────

        static void MapBooks(WebApplication app)
        {
            // GET /api/books?q=&cat= (public — guests allowed)
            app.MapGet("/api/books", async (HttpContext ctx) =>
            {
                // Public endpoint — no authentication required

                using var db = new LibraryDbContext();
                var query = db.Books.Include(b => b.Author).Include(b => b.Category).AsQueryable();

                var q   = ctx.Request.Query["q"].ToString();
                var cat = ctx.Request.Query["cat"].ToString();

                if (!string.IsNullOrWhiteSpace(q))
                    query = query.Where(b =>
                        b.Title.Contains(q) ||
                        (b.Author != null && b.Author.FullName.Contains(q)));

                if (!string.IsNullOrWhiteSpace(cat) && int.TryParse(cat, out int catId))
                    query = query.Where(b => b.CategoryId == catId);

                var books = await query.OrderBy(b => b.Title).ToListAsync();

                return Results.Ok(new
                {
                    books = books.Select(b => new
                    {
                        id             = b.Id,
                        title          = b.Title,
                        author         = b.Author?.FullName ?? "Bilinmiyor",
                        category       = b.Category?.Name ?? "—",
                        categoryId     = b.CategoryId,
                        available      = b.AvailableCopies > 0,
                        availableCopies= b.AvailableCopies,
                        totalCopies    = b.TotalCopies,
                        hasDigitalCopy = b.HasDigitalCopy,
                        averageRating  = Math.Round(b.AverageRating, 1),
                        hasCover       = !string.IsNullOrEmpty(b.CoverImagePath) && File.Exists(b.CoverImagePath)
                    })
                });
            });

            // GET /api/books/{id} (public — guests allowed)
            app.MapGet("/api/books/{id:int}", async (int id, HttpContext ctx) =>
            {
                var uid = GetUserId(ctx); // null for guests — favs/myRating won't load

                using var db = new LibraryDbContext();
                var book = await db.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .Include(b => b.Ratings).ThenInclude(r => r.User)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book is null) return Results.NotFound(new { error = "Kitap bulunamadı." });

                var myRating  = book.Ratings.FirstOrDefault(r => r.UserId == uid);
                bool isFav    = await db.UserFavorites.AnyAsync(f => f.UserId == uid && f.BookId == id);

                return Results.Ok(new
                {
                    id             = book.Id,
                    title          = book.Title,
                    author         = book.Author?.FullName ?? "—",
                    category       = book.Category?.Name ?? "—",
                    publisher      = book.Publisher ?? "—",
                    publishYear    = book.PublishYear?.ToString() ?? "—",
                    pageCount      = book.PageCount?.ToString() ?? "—",
                    language       = book.Language,
                    isbn           = book.ISBN,
                    description    = book.Description ?? "",
                    available      = book.AvailableCopies > 0,
                    availableCopies= book.AvailableCopies,
                    totalCopies    = book.TotalCopies,
                    hasDigitalCopy = book.HasDigitalCopy,
                    fileType       = (!string.IsNullOrEmpty(book.PdfFilePath) &&
                                      book.PdfFilePath.EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
                                      ? "epub" : "pdf",
                    averageRating  = Math.Round(book.AverageRating, 1),
                    ratingCount    = book.Ratings.Count,
                    isFavorite     = isFav,
                    hasCover       = !string.IsNullOrEmpty(book.CoverImagePath) && File.Exists(book.CoverImagePath),
                    myRating       = myRating is null ? null : new { score = myRating.Score, review = myRating.Review ?? "" },
                    ratings        = book.Ratings
                        .OrderByDescending(r => r.RatingDate)
                        .Take(20)
                        .Select(r => new
                        {
                            user   = $"{r.User?.FirstName} {r.User?.LastName}",
                            score  = r.Score,
                            review = r.Review ?? "",
                            date   = r.RatingDate.ToString("dd.MM.yyyy")
                        })
                });
            });
        }

        // ── CATEGORIES ─────────────────────────────────────────────────────────────────

        static void MapCategories(WebApplication app)
        {
            app.MapGet("/api/categories", async (HttpContext ctx) =>
            {
                // Public endpoint — no authentication required

                using var db = new LibraryDbContext();
                var cats = await db.Categories.OrderBy(c => c.Name).ToListAsync();
                return Results.Ok(cats.Select(c => new { id = c.Id, name = c.Name }));
            });

            // GET /api/authors (public)
            app.MapGet("/api/authors", async (HttpContext ctx) =>
            {
                // Public endpoint — no authentication required

                using var db = new LibraryDbContext();
                var authors = await db.Authors.OrderBy(a => a.FullName).ToListAsync();
                return Results.Ok(authors.Select(a => new { id = a.Id, name = a.FullName }));
            });
        }

        // ── FILE SERVING ─────────────────────────────────────────────────────────

        static void MapFiles(WebApplication app)
        {
            // GET /cover/{bookId}
            app.MapGet("/cover/{bookId:int}", async (int bookId, HttpContext ctx) =>
            {
                // Misafir modunda herkes kapak görseline erişebilir
                if (RequireAuthOrGuest(ctx) is IResult err) return err;

                using var db = new LibraryDbContext();
                var book = await db.Books.FindAsync(bookId);

                if (book is null || string.IsNullOrEmpty(book.CoverImagePath) || !File.Exists(book.CoverImagePath))
                    return Results.NotFound();

                var ext  = Path.GetExtension(book.CoverImagePath).ToLowerInvariant();
                var mime = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png"  => "image/png",
                    ".webp" => "image/webp",
                    _       => "image/jpeg"
                };
                var bytes = await File.ReadAllBytesAsync(book.CoverImagePath);
                return Results.File(bytes, mime);
            });

            // GET /pdf/{bookId} — Tarayıcıda inline görüntülemek için (indirmez)
            app.MapGet("/pdf/{bookId:int}", async (int bookId, HttpContext ctx) =>
            {
                // Misafir modunda oturumsuz da erişilebilir; okuma geçmişi sadece oturum varsa kaydedilir
                if (RequireAuthOrGuest(ctx) is IResult err) return err;
                var uid = GetUserId(ctx); // misafir modunda null olabilir

                try
                {
                    using var db = new LibraryDbContext();
                    var book = await db.Books.FindAsync(bookId);

                    if (book is null || !book.HasDigitalCopy ||
                        string.IsNullOrEmpty(book.PdfFilePath) || !File.Exists(book.PdfFilePath))
                        return Results.NotFound(new { error = "PDF dosyası bulunamadı." });

                    // Okuma geçmişini yalnızca oturum açık kullanıcılar için kaydet
                    if (uid is not null)
                    {
                        try
                        {
                            var hist = await db.ReadingHistories
                                .FirstOrDefaultAsync(h => h.UserId == uid && h.BookId == bookId);
                            if (hist is null)
                            {
                                db.ReadingHistories.Add(new ReadingHistory
                                {
                                    UserId = uid.Value, BookId = bookId,
                                    StartDate = DateTime.Now, LastReadDate = DateTime.Now
                                });
                            }
                            else { hist.LastReadDate = DateTime.Now; }
                            await db.SaveChangesAsync();
                        }
                        catch (Exception hx) { WebServer.AddLog($"[UYARI] Geçmiş: {hx.Message}"); }
                    }

                    WebServer.AddLog($"PDF okundu: {book.Title} (uid={uid?.ToString() ?? "misafir"})");

                    // FileStream — streamed without loading into memory, supports range requests
                    var fileStream = new FileStream(
                        book.PdfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    // No fileDownloadName → Content-Disposition: inline → browser renders in-page
                    return Results.Stream(fileStream, "application/pdf",
                        enableRangeProcessing: true);
                }
                catch (Exception ex)
                {
                    WebServer.AddLog($"[HATA] PDF: {ex.Message}");
                    return Results.Problem(detail: ex.Message, statusCode: 500);
                }
            });

            // GET /epub/{bookId} — EPUB dosyasını bibi viewer için sun
            app.MapGet("/epub/{bookId:int}", async (int bookId, HttpContext ctx) =>
            {
                // Misafir modunda oturumsuz da erişilebilir; okuma geçmişi sadece oturum varsa kaydedilir
                if (RequireAuthOrGuest(ctx) is IResult err) return err;
                var uid = GetUserId(ctx); // misafir modunda null olabilir

                try
                {
                    using var db = new LibraryDbContext();
                    var book = await db.Books.FindAsync(bookId);

                    if (book is null || !book.HasDigitalCopy ||
                        string.IsNullOrEmpty(book.PdfFilePath) || !File.Exists(book.PdfFilePath))
                        return Results.NotFound(new { error = "EPUB dosyası bulunamadı." });

                    // Okuma geçmişini yalnızca oturum açık kullanıcılar için kaydet
                    if (uid is not null)
                    {
                        try
                        {
                            var hist = await db.ReadingHistories
                                .FirstOrDefaultAsync(h => h.UserId == uid && h.BookId == bookId);
                            if (hist is null)
                            {
                                db.ReadingHistories.Add(new ReadingHistory
                                {
                                    UserId = uid.Value, BookId = bookId,
                                    StartDate = DateTime.Now, LastReadDate = DateTime.Now
                                });
                            }
                            else { hist.LastReadDate = DateTime.Now; }
                            await db.SaveChangesAsync();
                        }
                        catch { }
                    }

                    WebServer.AddLog($"EPUB okundu: {book.Title} (uid={uid?.ToString() ?? "misafir"})");

                    var fileStream = new FileStream(
                        book.PdfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return Results.Stream(fileStream, "application/epub+zip",
                        enableRangeProcessing: true);
                }
                catch (Exception ex)
                {
                    WebServer.AddLog($"[HATA] EPUB: {ex.Message}");
                    return Results.Problem(detail: ex.Message, statusCode: 500);
                }
            });

            // GET /api/books/{id}/progress — Kullanıcının okuma konumunu getir
            app.MapGet("/api/books/{id:int}/progress", async (int id, HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                using var db = new LibraryDbContext();
                var hist = await db.ReadingHistories
                    .FirstOrDefaultAsync(h => h.UserId == uid && h.BookId == id);

                return Results.Ok(new
                {
                    currentPage  = hist?.CurrentPage ?? 1,
                    lastLocation = hist?.LastLocationCfi ?? "",
                    lastReadDate = hist?.LastReadDate.ToString("dd.MM.yyyy HH:mm") ?? ""
                });
            });

            // POST /api/books/{id}/progress — Okuma konumunu kaydet
            app.MapPost("/api/books/{id:int}/progress", async (int id, HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                var data = await ReadJson<ProgressDto>(ctx);
                if (data is null) return Results.BadRequest(new { error = "Geçersiz veri." });

                using var db = new LibraryDbContext();
                var hist = await db.ReadingHistories
                    .FirstOrDefaultAsync(h => h.UserId == uid && h.BookId == id);

                if (hist is null)
                {
                    hist = new ReadingHistory
                    {
                        UserId    = uid.Value,
                        BookId    = id,
                        StartDate = DateTime.Now
                    };
                    db.ReadingHistories.Add(hist);
                }

                if (data.Page.HasValue && data.Page.Value > 0)
                    hist.CurrentPage = data.Page.Value;

                if (!string.IsNullOrWhiteSpace(data.Cfi))
                    hist.LastLocationCfi = data.Cfi;

                hist.LastReadDate = DateTime.Now;

                await db.SaveChangesAsync();
                return Results.Ok(new { success = true });
            });
        }

        // ── ADMIN (Book CRUD) ─────────────────────────────────────────────────

        static void MapAdmin(WebApplication app)
        {
            // ── Yetki yardımcısı ──────────────────────────────────────────────────
            static async Task<(User? user, IResult? err)> RequirePerm(
                HttpContext ctx, Func<User, bool> check, string errMsg)
            {
                var uid = GetUserId(ctx);
                if (uid is null) return (null, Results.Json(new { error = "Giriş yapmanız gerekiyor." }, statusCode: 401));
                using var db2 = new LibraryDbContext();
                var u = await db2.Users.FindAsync(uid);
                if (u is null) return (null, Results.Json(new { error = "Kullanıcı bulunamadı." }, statusCode: 404));
                bool isAdmin = u.Role == UserRole.Admin;
                if (!isAdmin && !check(u))
                    return (null, Results.Json(new { error = errMsg }, statusCode: 403));
                return (u, null);
            }

            // POST /api/admin/books — Kitap Ekle
            app.MapPost("/api/admin/books", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Results.Json(new { error = "Giriş yapmanız gerekiyor." }, statusCode: 401);

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(uid);
                if (u is null) return Results.Json(new { error = "Kullanıcı bulunamadı." }, statusCode: 404);
                if (u.Role != UserRole.Admin && !u.CanAddBook)
                    return Results.Json(new { error = "Bu işlem için yetkiniz yok." }, statusCode: 403);

                var data = await ReadJson<BookWriteDto>(ctx);
                if (data is null || string.IsNullOrWhiteSpace(data.Title))
                    return Results.BadRequest(new { error = "Kitap adı zorunludur." });

                // ── Duplicate kontrol: aynı başlık + aynı yazar ────────────────
                int authorIdCheck = data.AuthorId ?? 0;
                if (authorIdCheck == 0 && !string.IsNullOrWhiteSpace(data.AuthorName))
                {
                    var existingAuthor = await db.Authors.FirstOrDefaultAsync(x => x.FullName == data.AuthorName.Trim());
                    authorIdCheck = existingAuthor?.Id ?? -1;
                }
                if (authorIdCheck > 0)
                {
                    bool alreadyExists = await db.Books.AnyAsync(b =>
                        b.Title.ToLower() == data.Title.Trim().ToLower() &&
                        b.AuthorId == authorIdCheck);
                    if (alreadyExists)
                        return Results.Json(
                            new { error = $"\"{ data.Title.Trim()}\" adlı kitap bu yazar için zaten kayıtlı." },
                            statusCode: 400);
                }

                // ── ISBN duplicate kontrolü (boş değilse) ─────────────────────
                var trimmedIsbn = data.Isbn?.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedIsbn))
                {
                    bool isbnExists = await db.Books.AnyAsync(b => b.ISBN == trimmedIsbn);
                    if (isbnExists)
                        return Results.Json(
                            new { error = $"ISBN numarası ({trimmedIsbn}) başka bir kitaba ait. Lütfen kontrol edin." },
                            statusCode: 400);
                }

                // Yazar: id ile ya da yeni isimle
                int authorId = data.AuthorId ?? 0;
                if (authorId == 0 && !string.IsNullOrWhiteSpace(data.AuthorName))
                {
                    var a = await db.Authors.FirstOrDefaultAsync(x => x.FullName == data.AuthorName.Trim())
                            ?? new Author { FullName = data.AuthorName.Trim() };
                    if (a.Id == 0) { db.Authors.Add(a); await db.SaveChangesAsync(); }
                    authorId = a.Id;
                }
                if (authorId == 0) return Results.BadRequest(new { error = "Yazar zorunludur." });

                try
                {
                    var book = new Book
                    {
                        Title           = data.Title.Trim(),
                        AuthorId        = authorId,
                        CategoryId      = data.CategoryId,
                        ISBN            = data.Isbn?.Trim() ?? string.Empty,
                        Publisher       = data.Publisher?.Trim(),
                        PublishYear     = data.PublishYear,
                        PageCount       = data.PageCount,
                        Language        = data.Language?.Trim() ?? "Türkçe",
                        Description     = data.Description?.Trim(),
                        TotalCopies     = Math.Max(1, data.TotalCopies),
                        AvailableCopies = Math.Max(1, data.TotalCopies),
                        Location        = data.Location?.Trim(),
                        AddedDate       = DateTime.Now
                    };
                    db.Books.Add(book);
                    await db.SaveChangesAsync();

                    WebServer.AddLog($"Kitap eklendi: {book.Title} (uid={uid})");
                    return Results.Ok(new { success = true, id = book.Id });
                }
                catch (Exception ex)
                {
                    // SQLite unique constraint veya başka DB hatası
                    var msg = ex.InnerException?.Message ?? ex.Message;
                    WebServer.AddLog($"[HATA] Kitap eklenemedi: {msg}");

                    if (msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("unique", StringComparison.OrdinalIgnoreCase))
                        return Results.Json(
                            new { error = "Bu kitap zaten kayıtlı (benzersizlik ihlali). Lütfen ISBN veya başlığı kontrol edin." },
                            statusCode: 400);

                    return Results.Json(new { error = "Kitap eklenirken bir hata oluştu. Lütfen tekrar deneyin." }, statusCode: 500);
                }
            });

            // PUT /api/admin/books/{id} — Kitap Düzenle
            app.MapPut("/api/admin/books/{id:int}", async (int id, HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Results.Json(new { error = "Giriş yapmanız gerekiyor." }, statusCode: 401);

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(uid);
                if (u is null) return Results.Json(new { error = "Kullanıcı bulunamadı." }, statusCode: 404);
                if (u.Role != UserRole.Admin && !u.CanEditBook)
                    return Results.Json(new { error = "Bu işlem için yetkiniz yok." }, statusCode: 403);

                var book = await db.Books.FindAsync(id);
                if (book is null) return Results.NotFound(new { error = "Kitap bulunamadı." });

                var data = await ReadJson<BookWriteDto>(ctx);
                if (data is null || string.IsNullOrWhiteSpace(data.Title))
                    return Results.BadRequest(new { error = "Kitap adı zorunludur." });

                int authorId = data.AuthorId ?? 0;
                if (authorId == 0 && !string.IsNullOrWhiteSpace(data.AuthorName))
                {
                    var a = await db.Authors.FirstOrDefaultAsync(x => x.FullName == data.AuthorName.Trim())
                            ?? new Author { FullName = data.AuthorName.Trim() };
                    if (a.Id == 0) { db.Authors.Add(a); await db.SaveChangesAsync(); }
                    authorId = a.Id;
                }
                if (authorId == 0) authorId = book.AuthorId; // değişmedi ise koru

                book.Title       = data.Title.Trim();
                book.AuthorId    = authorId;
                book.CategoryId  = data.CategoryId > 0 ? data.CategoryId : book.CategoryId;
                book.ISBN        = data.Isbn?.Trim() ?? book.ISBN;
                book.Publisher   = data.Publisher?.Trim() ?? book.Publisher;
                book.PublishYear = data.PublishYear ?? book.PublishYear;
                book.PageCount   = data.PageCount ?? book.PageCount;
                book.Language    = data.Language?.Trim() ?? book.Language;
                book.Description = data.Description?.Trim() ?? book.Description;
                book.Location    = data.Location?.Trim() ?? book.Location;
                if (data.TotalCopies > 0)
                {
                    int diff = data.TotalCopies - book.TotalCopies;
                    book.TotalCopies     = data.TotalCopies;
                    book.AvailableCopies = Math.Max(0, book.AvailableCopies + diff);
                }

                await db.SaveChangesAsync();
                WebServer.AddLog($"Kitap güncellendi: {book.Title} (uid={uid})");
                return Results.Ok(new { success = true });
            });

            // DELETE /api/admin/books/{id} — Kitap Sil
            app.MapDelete("/api/admin/books/{id:int}", async (int id, HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Results.Json(new { error = "Giriş yapmanız gerekiyor." }, statusCode: 401);

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(uid);
                if (u is null) return Results.Json(new { error = "Kullanıcı bulunamadı." }, statusCode: 404);
                if (u.Role != UserRole.Admin && !u.CanDeleteBook)
                    return Results.Json(new { error = "Bu işlem için yetkiniz yok." }, statusCode: 403);

                var book = await db.Books.FindAsync(id);
                if (book is null) return Results.NotFound(new { error = "Kitap bulunamadı." });

                // Aktif ödünç var mı kontrol et
                bool hasActiveLoans = await db.BookLoans.AnyAsync(l => l.BookId == id && l.ReturnDate == null);
                if (hasActiveLoans)
                    return Results.Json(new { error = "Bu kitabın aktif ödünç kaydı var. Önce iade işlemini tamamlayın." }, statusCode: 400);

                string title = book.Title;
                db.Books.Remove(book);
                await db.SaveChangesAsync();

                WebServer.AddLog($"Kitap silindi: {title} (uid={uid})");
                return Results.Ok(new { success = true });
            });

            // GET /api/admin/books/{id}/edit-data — Düzenleme formu için tam veri
            app.MapGet("/api/admin/books/{id:int}/edit-data", async (int id, HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Results.Json(new { error = "Giriş yapmanız gerekiyor." }, statusCode: 401);

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(uid);
                if (u is null || (u.Role != UserRole.Admin && !u.CanEditBook))
                    return Results.Json(new { error = "Yetki yok." }, statusCode: 403);

                var book = await db.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (book is null) return Results.NotFound(new { error = "Kitap bulunamadı." });

                return Results.Ok(new
                {
                    id           = book.Id,
                    title        = book.Title,
                    authorId     = book.AuthorId,
                    authorName   = book.Author?.FullName ?? "",
                    categoryId   = book.CategoryId,
                    isbn         = book.ISBN,
                    publisher    = book.Publisher ?? "",
                    publishYear  = book.PublishYear,
                    pageCount    = book.PageCount,
                    language     = book.Language,
                    description  = book.Description ?? "",
                    totalCopies  = book.TotalCopies,
                    location     = book.Location ?? ""
                });
            });
        }

        // ── SOCIAL (Ratings / Favorites) ──────────────────────────────────────

        static void MapSocial(WebApplication app)
        {
            // POST /api/books/{id}/rate
            app.MapPost("/api/books/{id:int}/rate", async (int id, HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                var data = await ReadJson<RatingDto>(ctx);
                if (data is null || data.Score < 1 || data.Score > 5)
                    return Results.BadRequest(new { error = "Puan 1-5 arasında olmalıdır." });

                using var db = new LibraryDbContext();
                var existing = await db.UserRatings
                    .FirstOrDefaultAsync(r => r.UserId == uid && r.BookId == id);

                if (existing is not null)
                {
                    existing.Score      = data.Score;
                    existing.Review     = data.Review;
                    existing.RatingDate = DateTime.Now;
                }
                else
                {
                    db.UserRatings.Add(new UserRating
                    {
                        UserId     = uid.Value,
                        BookId     = id,
                        Score      = data.Score,
                        Review     = data.Review,
                        RatingDate = DateTime.Now
                    });
                }
                await db.SaveChangesAsync();

                // Recalculate the book's average rating across all reviews
                var book = await db.Books.FindAsync(id);
                if (book is not null)
                {
                    var avg = await db.UserRatings
                        .Where(r => r.BookId == id)
                        .AverageAsync(r => (float)r.Score);
                    book.AverageRating = avg;
                    await db.SaveChangesAsync();
                }

                return Results.Ok(new { success = true });
            });

            // POST /api/books/{id}/favorite — toggle
            app.MapPost("/api/books/{id:int}/favorite", async (int id, HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                using var db = new LibraryDbContext();
                var existing = await db.UserFavorites
                    .FirstOrDefaultAsync(f => f.UserId == uid && f.BookId == id);

                bool added;
                if (existing is not null)
                {
                    db.UserFavorites.Remove(existing);
                    added = false;
                }
                else
                {
                    db.UserFavorites.Add(new UserFavorite { UserId = uid.Value, BookId = id });
                    added = true;
                }
                await db.SaveChangesAsync();

                return Results.Ok(new { success = true, isFavorite = added });
            });

            // GET /api/me/favorites
            app.MapGet("/api/me/favorites", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                using var db = new LibraryDbContext();
                var favs = await db.UserFavorites
                    .Include(f => f.Book).ThenInclude(b => b.Author)
                    .Where(f => f.UserId == uid)
                    .ToListAsync();

                return Results.Ok(favs.Select(f => new
                {
                    id        = f.Book.Id,
                    title     = f.Book.Title,
                    author    = f.Book.Author?.FullName ?? "—",
                    available = f.Book.AvailableCopies > 0
                }));
            });
        }

        // ── EXTRAS (History / Top Reads / Stats / Members) ─────────────────

        static void MapExtras(WebApplication app)
        {
            // GET /api/me/history  — Son okuduklarım (okuma geçmişi)
            app.MapGet("/api/me/history", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                using var db = new LibraryDbContext();
                var history = await db.ReadingHistories
                    .Include(h => h.Book).ThenInclude(b => b.Author)
                    .Where(h => h.UserId == uid)
                    .OrderByDescending(h => h.LastReadDate)
                    .Take(50)
                    .ToListAsync();

                return Results.Ok(history.Select(h => new
                {
                    bookId      = h.BookId,
                    title       = h.Book?.Title ?? "—",
                    author      = h.Book?.Author?.FullName ?? "—",
                    currentPage = h.CurrentPage,
                    lastReadDate = h.LastReadDate.ToString("dd.MM.yyyy HH:mm"),
                    hasCover    = !string.IsNullOrEmpty(h.Book?.CoverImagePath) && File.Exists(h.Book!.CoverImagePath),
                    hasDigitalCopy = h.Book?.HasDigitalCopy ?? false,
                    fileType    = !string.IsNullOrEmpty(h.Book?.PdfFilePath)
                                    ? System.IO.Path.GetExtension(h.Book.PdfFilePath).TrimStart('.').ToLower()
                                    : "pdf"
                }));
            });

            // GET /api/books/top-read  — En çok okunan kitaplar
            app.MapGet("/api/books/top-read", async (HttpContext ctx) =>
            {
                // Misafir modunda herkes görebilir
                if (RequireAuthOrGuest(ctx) is IResult err) return err;

                using var db = new LibraryDbContext();

                var topBooks = await db.ReadingHistories
                    .Include(h => h.Book).ThenInclude(b => b.Author)
                    .Include(h => h.Book).ThenInclude(b => b.Category)
                    .GroupBy(h => h.BookId)
                    .Select(g => new
                    {
                        BookId    = g.Key,
                        ReadCount = g.Count(),
                        Book      = g.First().Book
                    })
                    .OrderByDescending(x => x.ReadCount)
                    .Take(30)
                    .ToListAsync();

                return Results.Ok(topBooks.Select(x => new
                {
                    id        = x.BookId,
                    title     = x.Book?.Title ?? "—",
                    author    = x.Book?.Author?.FullName ?? "—",
                    category  = x.Book?.Category?.Name ?? "—",
                    readCount = x.ReadCount,
                    available = (x.Book?.AvailableCopies ?? 0) > 0,
                    hasDigitalCopy = x.Book?.HasDigitalCopy ?? false,
                    hasCover  = !string.IsNullOrEmpty(x.Book?.CoverImagePath) && File.Exists(x.Book!.CoverImagePath),
                    averageRating = x.Book?.AverageRating ?? 0.0
                }));
            });

            // GET /api/me/stats  — Kullanıcı istatistikleri
            app.MapGet("/api/me/stats", async (HttpContext ctx) =>
            {
                var uid = GetUserId(ctx);
                if (uid is null) return Unauthorized();

                using var db = new LibraryDbContext();

                var readCount  = await db.ReadingHistories.CountAsync(h => h.UserId == uid);
                var favCount   = await db.UserFavorites.CountAsync(f => f.UserId == uid);
                var rateCount  = await db.UserRatings.CountAsync(r => r.UserId == uid);

                return Results.Ok(new
                {
                    totalRead      = readCount,
                    totalFavorites = favCount,
                    totalRatings   = rateCount
                });
            });

            // GET /api/members — Tüm aktif üye listesi
            app.MapGet("/api/members", async (HttpContext ctx) =>
            {
                // Misafir modunda herkes üye listesini görebilir
                if (RequireAuthOrGuest(ctx) is IResult err) return err;

                using var db = new LibraryDbContext();

                var members = await db.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                    .ToListAsync();

                var result = new List<object>();
                foreach (var m in members)
                {
                    var readCount  = await db.ReadingHistories.CountAsync(h => h.UserId == m.Id);
                    var rateCount  = await db.UserRatings.CountAsync(r => r.UserId == m.Id);

                    // Most recently read book (latest reading history entry)
                    var current = await db.ReadingHistories
                        .Include(h => h.Book).ThenInclude(b => b.Author)
                        .Where(h => h.UserId == m.Id)
                        .OrderByDescending(h => h.LastReadDate)
                        .FirstOrDefaultAsync();

                    result.Add(new
                    {
                        id              = m.Id,
                        name            = $"{m.FirstName} {m.LastName}",
                        hasProfileImage = !string.IsNullOrEmpty(m.ProfileImagePath) && File.Exists(m.ProfileImagePath),
                        role            = m.Role.ToString(),
                        readCount,
                        rateCount,
                        currentlyReading = current is null ? null : (object)new
                        {
                            bookId = current.BookId,
                            title  = current.Book?.Title ?? "—",
                            author = current.Book?.Author?.FullName ?? "—"
                        }
                    });
                }

                return Results.Ok(result);
            });

            // GET /api/members/{id} — Tek üye genel profili
            app.MapGet("/api/members/{id:int}", async (int id, HttpContext ctx) =>
            {
                // Misafir modunda genel üye profili de görülebilir
                if (RequireAuthOrGuest(ctx) is IResult err) return err;

                using var db = new LibraryDbContext();
                var m = await db.Users.FindAsync(id);
                if (m is null || !m.IsActive)
                    return Results.NotFound(new { error = "Üye bulunamadı." });

                var readCount = await db.ReadingHistories.CountAsync(h => h.UserId == id);
                var rateCount = await db.UserRatings.CountAsync(r => r.UserId == id);

                // Most recently accessed book
                var current = await db.ReadingHistories
                    .Include(h => h.Book).ThenInclude(b => b.Author)
                    .Where(h => h.UserId == id)
                    .OrderByDescending(h => h.LastReadDate)
                    .FirstOrDefaultAsync();

                // Most recent reviews (up to 10)
                var reviews = await db.UserRatings
                    .Include(r => r.Book)
                    .Where(r => r.UserId == id && !string.IsNullOrEmpty(r.Review))
                    .OrderByDescending(r => r.RatingDate)
                    .Take(10)
                    .ToListAsync();

                return Results.Ok(new
                {
                    id              = m.Id,
                    name            = $"{m.FirstName} {m.LastName}",
                    hasProfileImage = !string.IsNullOrEmpty(m.ProfileImagePath) && File.Exists(m.ProfileImagePath),
                    role            = m.Role.ToString(),
                    registrationDate = m.RegistrationDate.ToString("MMMM yyyy"),
                    readCount,
                    rateCount,
                    currentlyReading = current is null ? null : (object)new
                    {
                        bookId = current.BookId,
                        title  = current.Book?.Title ?? "—",
                        author = current.Book?.Author?.FullName ?? "—",
                        lastReadDate = current.LastReadDate.ToString("dd.MM.yyyy")
                    },
                    recentReviews = reviews.Select(r => new
                    {
                        bookId = r.BookId,
                        title  = r.Book?.Title ?? "—",
                        score  = r.Score,
                        review = r.Review ?? "",
                        date   = r.RatingDate.ToString("dd.MM.yyyy")
                    })
                });
            });

            // GET /api/users/{id}/avatar — Başka üyenin avatarını getir
            app.MapGet("/api/users/{id:int}/avatar", async (int id, HttpContext ctx) =>
            {
                // Misafir modunda avatara erişim açık
                if (RequireAuthOrGuest(ctx) is IResult err) return err;

                using var db = new LibraryDbContext();
                var u = await db.Users.FindAsync(id);
                if (u is null || string.IsNullOrEmpty(u.ProfileImagePath) || !File.Exists(u.ProfileImagePath))
                    return Results.Redirect("/favicon.ico");

                var ext  = Path.GetExtension(u.ProfileImagePath).ToLowerInvariant();
                var mime = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png"  => "image/png",
                    ".webp" => "image/webp",
                    ".bmp"  => "image/bmp",
                    _       => "image/jpeg"
                };
                return Results.File(u.ProfileImagePath, mime);
            });
        }

        // ── DTOs ──────────────────────────────────────────────────────────────

        record LoginDto(string Email, string Password);
        record RegisterDto(string FirstName, string LastName, string Email, string Password);
        record RatingDto(int Score, string? Review);
        record ProgressDto(int? Page, string? Cfi); // PDF sayfa veya EPUB CFI konumu
        record BookWriteDto(
            string Title,
            int? AuthorId,
            string? AuthorName,
            int CategoryId,
            string? Isbn,
            string? Publisher,
            int? PublishYear,
            int? PageCount,
            string? Language,
            string? Description,
            int TotalCopies,
            string? Location
        );
        record ProfileUpdateDto(string FirstName, string LastName, string? Phone);
        record PasswordUpdateDto(string CurrentPassword, string NewPassword);

        static object MapUserDto(User u) => new
        {
            id           = u.Id,
            firstName    = u.FirstName,
            lastName     = u.LastName,
            name         = $"{u.FirstName} {u.LastName}",
            email        = u.Email,
            phone        = u.Phone ?? "",
            role         = u.Role.ToString(),
            canAddBook   = u.CanAddBook,
            canEditBook  = u.CanEditBook,
            canDeleteBook= u.CanDeleteBook,
            hasProfileImage = !string.IsNullOrEmpty(u.ProfileImagePath) && File.Exists(u.ProfileImagePath)
        };
    }
}
