using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KitapCell.Data;
using KitapCell.Models;
using KitapCell.Repositories;
using KitapCell.Core;
using Microsoft.Web.WebView2.Core;

namespace KitapCell
{
    public partial class BookReaderForm : Form
    {
        private readonly Book _book;
        private readonly LibraryDbContext _dbContext;
        private readonly Repository<ReadingHistory> _historyRepo;
        private ReadingHistory? _session;

        private DateTime _sessionStart = DateTime.Now;
        private int _lastKnownPage = 1;
        private string? _lastKnownCfi = null;
        private System.Windows.Forms.Timer _trackingTimer;

        public BookReaderForm(Book book)
        {
            _book = book;
            _dbContext = new LibraryDbContext();
            _historyRepo = new Repository<ReadingHistory>(_dbContext);

            InitializeComponent();
            ThemeHelper.Apply(this);

            this.Text = $"📖  {_book.Title}";
            lblTitle.Text = $"📖  {_book.Title}";

            // Form Sürükleme (Drag) Event'leri
            pnlTopBar.MouseDown += TopBar_MouseDown;
            lblTitle.MouseDown += TopBar_MouseDown;
            lblPageInfo.MouseDown += TopBar_MouseDown;
        }

        // ── Form Sürükleme (Borderless Drag) ───────────────────────────────────
        
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xA1, 0x2, 0);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x40000; // WS_THICKFRAME (Yeniden boyutlandırmaya izin verir)
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int RESIZE_HANDLE_SIZE = 10;
            
            base.WndProc(ref m);
            
            if (m.Msg == WM_NCHITTEST && (int)m.Result == 0x1) // HTCLIENT
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    Point screenPoint = new Point(m.LParam.ToInt32());
                    Point clientPoint = this.PointToClient(screenPoint);

                    if (clientPoint.X <= RESIZE_HANDLE_SIZE && clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)13; // HTTOPLEFT
                    else if (clientPoint.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE && clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)14; // HTTOPRIGHT
                    else if (clientPoint.X <= RESIZE_HANDLE_SIZE && clientPoint.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)16; // HTBOTTOMLEFT
                    else if (clientPoint.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE && clientPoint.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    else if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)12; // HTTOP
                    else if (clientPoint.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)15; // HTBOTTOM
                    else if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)10; // HTLEFT
                    else if (clientPoint.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE)
                        m.Result = (IntPtr)11; // HTRIGHT
                }
            }
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await InitWebViewAsync();
            await LoadOrCreateSessionAsync();
        }

        // ── WebView2 ──────────────────────────────────────────────────────────

        private async Task InitWebViewAsync()
        {
            try
            {
                // Windows 10 uyumluluğu: explicit kullanıcı veri klasörü belirt
                // null kullanmak bazen Win10'da kilit sorunlarına yol açıyor
                string webViewDataDir = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                    "KitapCell", "WebView2Cache");
                System.IO.Directory.CreateDirectory(webViewDataDir);

                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(
                    null, webViewDataDir);
                await webView.EnsureCoreWebView2Async(env);

                // Yerel dosyalar (Assets vb.) için Sanal HTTP Sunucu
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "library.app", 
                    AppDomain.CurrentDomain.BaseDirectory, 
                    Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow
                );

                // Alternatif URL takibi
                webView.CoreWebView2.SourceChanged += (s, e) => TrackPageFromUrl();

                // Güvenli sayfa takibi (Scroll sırasında hash değişimi yakalama)
                _trackingTimer = new System.Windows.Forms.Timer { Interval = 1500 };
                _trackingTimer.Tick += async (s, e) => await TrackPageFromJSAsync();
                _trackingTimer.Start();
            }
            catch (Exception ex)
            {
                // WebView2 başlatılamadıysa PDF için sistem okuyucusuna fallback yap
                string ext = System.IO.Path.GetExtension(_book.PdfFilePath ?? "").ToLower();
                if (ext == ".pdf")
                {
                    var result = MessageBox.Show(
                        "Dahili PDF okuyucu başlatılamadı (WebView2 sorunu).\n\n" +
                        "Hata: " + ex.Message +
                        "\n\nPDF sisteminizin varsayılan okuyucusunda açılsın mı?",
                        "Okuyucu Hatası", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(
                                new System.Diagnostics.ProcessStartInfo(_book.PdfFilePath) { UseShellExecute = true });
                        }
                        catch { }
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Dahili okuyucu başlatılamadı.\n\nHata: " + ex.Message +
                        "\n\nMicrosoft Edge WebView2 Runtime yüklü olduğundan emin olun:\nhttps://developer.microsoft.com/microsoft-edge/webview2\n\n" +
                        "Kurulum için: winget install Microsoft.EdgeWebView2Runtime",
                        "Okuyucu Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            }
        }

        private void TrackPageFromUrl()
        {
            var url = webView.CoreWebView2?.Source ?? "";
            ParsePageFromHash(url);
        }

        private async Task TrackPageFromJSAsync()
        {
            if (webView.CoreWebView2 == null) return;
            try
            {
                string ext = System.IO.Path.GetExtension(_book.PdfFilePath).ToLower();
                
                if (ext == ".pdf")
                {
                    string js = "window.PDFViewerApplication ? window.PDFViewerApplication.page.toString() : null";
                    string result = await webView.CoreWebView2.ExecuteScriptAsync(js);
                    
                    if (!string.IsNullOrEmpty(result) && result != "null")
                    {
                        result = result.Trim('"');
                        if (int.TryParse(result, out int page) && page > 0)
                        {
                            if (_lastKnownPage != page)
                            {
                                _lastKnownPage = page;
                                lblPageInfo.Text = $"Sayfa: {page}";
                            }
                        }
                    }
                    else
                    {
                        string hash = await webView.CoreWebView2.ExecuteScriptAsync("window.location.hash");
                        if (!string.IsNullOrEmpty(hash) && hash != "null")
                        {
                            ParsePageFromHash(hash.Trim('"'));
                        }
                    }
                }
                else if (ext == ".epub")
                {
                    string js = "window.location.hash ? window.location.hash : null";
                    string hash = await webView.CoreWebView2.ExecuteScriptAsync(js);
                    if (!string.IsNullOrEmpty(hash) && hash != "null")
                    {
                        hash = hash.Trim('"');
                        if (hash.StartsWith("#epubcfi"))
                        {
                            string cfi = hash.Substring(1); // baştaki '#' karakterini atıyoruz
                            if (_lastKnownCfi != cfi)
                            {
                                _lastKnownCfi = cfi;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void ParsePageFromHash(string source)
        {
            if (source.Contains("#page="))
            {
                var pageStr = source.Split(new[] { "#page=" }, StringSplitOptions.None).LastOrDefault();
                if (pageStr != null && int.TryParse(pageStr.Split('&')[0], out int page) && page > 0)
                {
                    if (_lastKnownPage != page)
                    {
                        _lastKnownPage = page;
                        lblPageInfo.Text = $"Sayfa: {page}";
                    }
                }
            }
        }

        // ── Okuma Geçmişi ─────────────────────────────────────────────────────

        private async Task LoadOrCreateSessionAsync()
        {
            if (Core.GlobalSession.CurrentUser == null)
            {
                lblSessionInfo.Text = "⚠️ Giriş yapılmadı — ilerleme kaydedilmeyecek";
                lblSessionInfo.ForeColor = System.Drawing.Color.FromArgb(245, 158, 11);
                // Giriş yapılmasa da EPUB/PDF'i aç
                OpenBook(1, null);
                return;
            }

            int userId = Core.GlobalSession.CurrentUser.Id;

            var allHistory = await _historyRepo.GetAllAsync();
            _session = allHistory.FirstOrDefault(h => h.UserId == userId && h.BookId == _book.Id);

            if (_session == null)
            {
                _session = new ReadingHistory
                {
                    UserId = userId,
                    BookId = _book.Id,
                    CurrentPage = 1,
                    StartDate = DateTime.Now,
                    LastReadDate = DateTime.Now
                };
                await _historyRepo.AddAsync(_session);
            }

            _lastKnownPage = _session.CurrentPage > 0 ? _session.CurrentPage : 1;
            _lastKnownCfi = _session.LastLocationCfi;

            lblSessionInfo.Text = $"Son okuma: {_session.LastReadDate:dd.MM.yyyy HH:mm}";
            string ext = System.IO.Path.GetExtension(_book.PdfFilePath).ToLower();
            lblPageInfo.Text = ext == ".epub" ? "E-Kitap Okunuyor" : $"Sayfa: {_lastKnownPage}";

            OpenBook(_lastKnownPage, _lastKnownCfi);
        }

        private void OpenBook(int page, string? cfi)
        {
            if (string.IsNullOrEmpty(_book.PdfFilePath) || !System.IO.File.Exists(_book.PdfFilePath))
            {
                MessageBox.Show("Dijital dosya bulunamadı.", "Dosya Yok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            string ext = System.IO.Path.GetExtension(_book.PdfFilePath).ToLower();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string relativeDoc = _book.PdfFilePath;
            
            if (relativeDoc.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                relativeDoc = relativeDoc.Substring(baseDir.Length);
            
            relativeDoc = relativeDoc.Replace('\\', '/');
            string fileUrl = $"http://library.app/{relativeDoc}";

            if (ext == ".pdf")
            {
                string viewerUrl = $"http://library.app/Assets/pdfjs/web/viewer.html?file={Uri.EscapeDataString(fileUrl)}#page={page}";
                webView.CoreWebView2.Navigate(viewerUrl);
            }
            else if (ext == ".epub")
            {
                // Bibi Viewer rotası
                string viewerUrl = $"http://library.app/Assets/bibi/bibi/index.html?book={Uri.EscapeDataString(fileUrl)}";
                
                // Eğer daha önce kaydedilen lokasyon varsa URL'ye hashtag olarak ekliyoruz. Örn: #epubcfi(/6/12...)
                if (!string.IsNullOrEmpty(cfi))
                {
                    viewerUrl += $"#{cfi}";
                }
                
                webView.CoreWebView2.Navigate(viewerUrl);
            }
            else 
            {
                MessageBox.Show("Desteklenmeyen dijital dosya formatı.", "Uyumsuz Format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void SaveSession()
        {
            if (_session == null || Core.GlobalSession.CurrentUser == null) return;

            int elapsed = (int)(DateTime.Now - _sessionStart).TotalSeconds;

            string ext = System.IO.Path.GetExtension(_book.PdfFilePath).ToLower();
            if (ext == ".epub")
            {
                _session.LastLocationCfi = _lastKnownCfi;
            }
            else 
            {
                _session.CurrentPage = _lastKnownPage;
            }

            _session.LastReadDate = DateTime.Now;
            _session.TotalReadSeconds += elapsed;

            // Kapanışta async yerine senkron kaydediyoruz çünkü _dbContext Dispose olabilir
            _dbContext.ReadingHistories.Update(_session);
            _dbContext.SaveChanges();
        }

        // ── Form Kapatma ──────────────────────────────────────────────────────

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveSession();
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _trackingTimer?.Stop();
            _trackingTimer?.Dispose();
            _dbContext?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
