using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KitapCell.Data;
using KitapCell.Models;
using KitapCell.Repositories;
using KitapCell.Core;

namespace KitapCell
{
    /// <summary>
    /// Dialog form used to add a new book or edit an existing one in the library catalog.
    /// Supports manual data entry and automatic metadata fetching via ISBN lookup
    /// (Open Library API → Google Books API fallback).
    /// Also supports USB barcode scanner input: when the scanner sends an Enter key after
    /// writing the ISBN into <c>txtISBN</c>, the lookup is triggered automatically.
    /// Cover images can be selected manually or extracted from the first PDF page.
    /// </summary>
    public partial class AddBookForm : Form
    {
        /// <summary>True when the form is opened for editing an existing book title; false for adding a new one.</summary>
        private bool isEdit;

        /// <summary>EF Core database context for the lifetime of this form.</summary>
        private LibraryDbContext _dbContext;

        /// <summary>Repository for book CRUD operations.</summary>
        private BookRepository _bookRepo;

        /// <summary>Repository for category read operations (populates the drop-down).</summary>
        private Repository<Category> _categoryRepo;

        /// <summary>Repository for author lookup and creation.</summary>
        private Repository<Author> _authorRepo;

        /// <summary>Full path to the cover image selected by the user (empty if none selected).</summary>
        private string _selectedCoverPath = string.Empty;

        /// <summary>Full path to the PDF or EPUB file selected by the user (empty if none selected).</summary>
        private string _selectedPdfPath = string.Empty;

        public AddBookForm(string existingTitle = "")
        {
            isEdit = !string.IsNullOrEmpty(existingTitle);
            InitializeComponent();
            ThemeHelper.Apply(this);
            
            _dbContext = new LibraryDbContext();
            _bookRepo = new BookRepository(_dbContext);
            _categoryRepo = new Repository<Category>(_dbContext);
            _authorRepo = new Repository<Author>(_dbContext);

            if (isEdit)
            {
                this.Text = "Kitap Düzenle";
                lblFormTitle.Text = "✏️  Kitap Düzenle";
                btnKaydet.Text = "💾  Güncelle";
                txtBaslik.Text = existingTitle;
            }

            // Olayları Bağla
            this.Load += AddBookForm_Load;
            btnSelectCover.Click += BtnSelectCover_Click;
            btnSelectPdf.Click += BtnSelectPdf_Click;
            // Barkod okuyucu Enter göndererek ISBN alanını tetikler
            txtISBN.KeyDown += TxtISBN_KeyDown;
        }

        private async void AddBookForm_Load(object sender, EventArgs e)
        {
            // Kategorileri Veritabanından Çekip ComboBox'a aktar
            var categories = await _categoryRepo.GetAllAsync();
            cmbKategori.DataSource = categories.ToList();
            cmbKategori.DisplayMember = "Name";
            cmbKategori.ValueMember = "Id";
        }

        private void BtnSelectCover_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Kitap Kapağı Seçin";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedCoverPath = ofd.FileName;
                    picCover.ImageLocation = _selectedCoverPath;
                }
            }
        }

        private void BtnSelectPdf_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Dijital Kitap (*.pdf;*.epub)|*.pdf;*.epub";
                ofd.Title = "Kitabın Dijital Kopyasını (PDF/EPUB) Seçin";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedPdfPath = ofd.FileName;
                    txtPdfPath.Text = Path.GetFileName(_selectedPdfPath);

                    // Başlık alanı boşsa PDF dosya adını (uzantısız) otomatik doldur
                    if (string.IsNullOrWhiteSpace(txtBaslik.Text))
                        txtBaslik.Text = Path.GetFileNameWithoutExtension(_selectedPdfPath);
                }
            }
        }

        private async void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBaslik.Text))
            { MessageBox.Show("Kitap başlığı boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            // 1. Yazarı Bul veya Yeni Oluştur (yazar belirtilmemişse "Bilinmiyor" kullan)
            string yazarAdi = string.IsNullOrWhiteSpace(txtYazar.Text) ? "Bilinmiyor" : txtYazar.Text.Trim();
            var authors = await _authorRepo.GetAllAsync();
            var author = authors.FirstOrDefault(a => a.FullName.Equals(yazarAdi, StringComparison.OrdinalIgnoreCase));
            
            if (author == null)
            {
                author = new Author { FullName = yazarAdi, Biography = "Sistem tarafından otomatik eklendi." };
                await _authorRepo.AddAsync(author);
            }

            // 2. Dosyaları kopyalayacağımız "Assets" klasörü altyapısı
            string assetsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            string coversFolder = Path.Combine(assetsFolder, "Covers");
            string pdfsFolder = Path.Combine(assetsFolder, "Pdfs");
            
            if (!Directory.Exists(coversFolder)) Directory.CreateDirectory(coversFolder);
            if (!Directory.Exists(pdfsFolder)) Directory.CreateDirectory(pdfsFolder);

            string savedCoverPath = null;
            string savedPdfPath = null;

            // Kapak resmi kopyala
            if (!string.IsNullOrEmpty(_selectedCoverPath) && File.Exists(_selectedCoverPath))
            {
                string ext = Path.GetExtension(_selectedCoverPath);
                string newFileName = Guid.NewGuid().ToString() + ext;
                savedCoverPath = Path.Combine(coversFolder, newFileName);
                File.Copy(_selectedCoverPath, savedCoverPath, true);
            }

            // PDF veya EPUB kopyala
            if (!string.IsNullOrEmpty(_selectedPdfPath) && File.Exists(_selectedPdfPath))
            {
                string ext = Path.GetExtension(_selectedPdfPath);
                string newFileName = Guid.NewGuid().ToString() + ext;
                savedPdfPath = Path.Combine(pdfsFolder, newFileName);
                File.Copy(_selectedPdfPath, savedPdfPath, true);
            }

            // Otomatik Kapak Çıkarımı (Sadece PDF ve kapak elle seçilmemişse)
            if (string.IsNullOrEmpty(savedCoverPath) && !string.IsNullOrEmpty(savedPdfPath))
            {
                if (Path.GetExtension(savedPdfPath).ToLower() == ".pdf")
                {
                    try
                    {
                        using (var pdfDoc = PdfiumViewer.PdfDocument.Load(savedPdfPath))
                        {
                            if (pdfDoc.PageCount > 0)
                            {
                                // İlk sayfayı bitmap olarak render et (150 DPI)
                                using (var img = pdfDoc.Render(0, 150, 150, true))
                                {
                                    string cvrFileName = Guid.NewGuid().ToString() + "_auto.png";
                                    savedCoverPath = Path.Combine(coversFolder, cvrFileName);
                                    img.Save(savedCoverPath, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Otomatik kapak çıkarılamadı: " + ex.Message);
                    }
                }
            }

            int categoryId = cmbKategori.SelectedValue != null ? (int)cmbKategori.SelectedValue : 1; 

            int topKopya = 1;
            int.TryParse(txtKopya.Text, out topKopya);

            // 3. Yeni Kitap Nesnesi Oluştur
            var newBook = new Book
            {
                Title = txtBaslik.Text.Trim(),
                AuthorId = author.Id,
                CategoryId = categoryId,
                ISBN = string.IsNullOrWhiteSpace(txtISBN.Text) ? "DIGITAL-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper() : txtISBN.Text.Trim(),
                Publisher = txtYayinci.Text.Trim(),
                TotalCopies = topKopya,
                AvailableCopies = topKopya,
                CoverImagePath = savedCoverPath,
                PdfFilePath = savedPdfPath,
                HasDigitalCopy = !string.IsNullOrEmpty(savedPdfPath)
            };

            if (int.TryParse(txtYil.Text, out int yayinYili))
                newBook.PublishYear = yayinYili;

            // 4. Veritabanına Ekle
            await _bookRepo.AddAsync(newBook);

            string msg = $"'{txtBaslik.Text}' kitabı kütüphaneye başarıyla eklendi!";
            MessageBox.Show(msg, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnIptal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; 
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _dbContext?.Dispose();
            base.OnFormClosed(e);
        }

        /// <summary>
        /// KeyDown handler for the ISBN text box.
        /// When a USB barcode scanner finishes reading it automatically sends an Enter key;
        /// this handler intercepts that key press and triggers the ISBN API lookup,
        /// eliminating the need to click the search button manually.
        /// <see cref="System.Windows.Forms.KeyEventArgs.SuppressKeyPress"/> is set to true
        /// to prevent the default Enter-key behaviour (e.g. activating the Accept button).
        /// </summary>
        private void TxtISBN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Bip sesini engelle
                BtnSearchIsbn_Click(sender, EventArgs.Empty);
            }
        }

        private async void BtnSearchIsbn_Click(object sender, EventArgs e)
        {
            string isbn = txtISBN.Text.Trim();
            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show("Lütfen önce bir ISBN numarası girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSearchIsbn.Enabled = false;
            btnSearchIsbn.Text = "⏳..";

            try
            {
                var bookData = await KitapCell.Services.BookFetchService.FetchBookByIsbnAsync(isbn);
                
                if (bookData == null)
                {
                    MessageBox.Show("Kitap bulunamadı! Lütfen bilgileri manuel giriniz.", "Sonuç Yok", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // UI alanlarını doldur
                if (!string.IsNullOrWhiteSpace(bookData.Title))
                    txtBaslik.Text = bookData.Title;

                if (bookData.Authors.Any())
                    txtYazar.Text = string.Join(", ", bookData.Authors);

                if (!string.IsNullOrWhiteSpace(bookData.Publisher))
                    txtYayinci.Text = bookData.Publisher;

                if (!string.IsNullOrWhiteSpace(bookData.PublishYear))
                    txtYil.Text = bookData.PublishYear;

                if (bookData.PageCount > 0)
                {
                    txtAciklama.Text = $"Sayfa Sayısı: {bookData.PageCount}\n(Otomatik API ile getirildi)";
                }

                if (!string.IsNullOrWhiteSpace(bookData.CoverUrl))
                {
                    try
                    {
                        using var client = new HttpClient();
                        byte[] imageBytes = await client.GetByteArrayAsync(bookData.CoverUrl);
                        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");
                        File.WriteAllBytes(tempPath, imageBytes);
                        
                        _selectedCoverPath = tempPath;
                        picCover.ImageLocation = _selectedCoverPath;
                        picCover.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch { /* Kapak resmi inmezse sessizce geç. */ }
                }

                txtKopya.Text = "1"; // Varsayılan değer
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSearchIsbn.Enabled = true;
                btnSearchIsbn.Text = "Ara 🔍";
            }
        }
    }
}
