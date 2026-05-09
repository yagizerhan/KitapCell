namespace KitapCell
{
    partial class AddBookForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlAccent = new System.Windows.Forms.Panel();
            this.lblFormTitle = new System.Windows.Forms.Label();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.lblBaslik = new System.Windows.Forms.Label();
            this.txtBaslik = new System.Windows.Forms.TextBox();
            this.lblYazar = new System.Windows.Forms.Label();
            this.txtYazar = new System.Windows.Forms.TextBox();
            this.lblKategori = new System.Windows.Forms.Label();
            this.lblYayinci = new System.Windows.Forms.Label();
            this.txtYayinci = new System.Windows.Forms.TextBox();
            this.lblYil = new System.Windows.Forms.Label();
            this.txtYil = new System.Windows.Forms.TextBox();
            this.lblISBN = new System.Windows.Forms.Label();
            this.txtISBN = new System.Windows.Forms.TextBox();
            this.lblKopya = new System.Windows.Forms.Label();
            this.txtKopya = new System.Windows.Forms.TextBox();
            this.lblAciklama = new System.Windows.Forms.Label();
            this.txtAciklama = new System.Windows.Forms.TextBox();
            this.btnIptal = new System.Windows.Forms.Button();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.cmbKategori = new System.Windows.Forms.ComboBox();
            this.picCover = new System.Windows.Forms.PictureBox();
            this.btnSelectCover = new System.Windows.Forms.Button();
            this.lblPdfEkle = new System.Windows.Forms.Label();
            this.txtPdfPath = new System.Windows.Forms.TextBox();
            this.btnSelectPdf = new System.Windows.Forms.Button();
            this.btnSearchIsbn = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).BeginInit();
            this.pnlBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.pnlHeader.Controls.Add(this.pnlAccent);
            this.pnlHeader.Controls.Add(this.lblFormTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(800, 64);
            this.pnlHeader.TabIndex = 0;
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.pnlAccent.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlAccent.Location = new System.Drawing.Point(0, 0);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(4, 64);
            this.pnlAccent.TabIndex = 0;
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblFormTitle.ForeColor = System.Drawing.Color.White;
            this.lblFormTitle.Location = new System.Drawing.Point(20, 18);
            this.lblFormTitle.Name = "lblFormTitle";
            this.lblFormTitle.Size = new System.Drawing.Size(400, 30);
            this.lblFormTitle.TabIndex = 1;
            this.lblFormTitle.Text = "📚  Yeni Kitap Ekle";
            // 
            // pnlBody
            // 
            this.pnlBody.AutoScroll = true;
            this.pnlBody.BackColor = System.Drawing.Color.Transparent;
            this.pnlBody.Controls.Add(this.lblBaslik);
            this.pnlBody.Controls.Add(this.txtBaslik);
            this.pnlBody.Controls.Add(this.lblYazar);
            this.pnlBody.Controls.Add(this.txtYazar);
            this.pnlBody.Controls.Add(this.lblKategori);
            this.pnlBody.Controls.Add(this.cmbKategori); // textbox yerine combobox
            this.pnlBody.Controls.Add(this.lblYayinci);
            this.pnlBody.Controls.Add(this.txtYayinci);
            this.pnlBody.Controls.Add(this.lblYil);
            this.pnlBody.Controls.Add(this.txtYil);
            this.pnlBody.Controls.Add(this.lblISBN);
            this.pnlBody.Controls.Add(this.txtISBN);
            this.pnlBody.Controls.Add(this.lblKopya);
            this.pnlBody.Controls.Add(this.txtKopya);
            this.pnlBody.Controls.Add(this.lblAciklama);
            this.pnlBody.Controls.Add(this.txtAciklama);
            this.pnlBody.Controls.Add(this.btnSearchIsbn);

            // Sağ Taraf Elemanları
            this.pnlBody.Controls.Add(this.picCover);
            this.pnlBody.Controls.Add(this.btnSelectCover);
            this.pnlBody.Controls.Add(this.lblPdfEkle);
            this.pnlBody.Controls.Add(this.txtPdfPath);
            this.pnlBody.Controls.Add(this.btnSelectPdf);

            this.pnlBody.Controls.Add(this.btnIptal);
            this.pnlBody.Controls.Add(this.btnKaydet);
            this.pnlBody.Location = new System.Drawing.Point(0, 64);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(800, 464);
            this.pnlBody.TabIndex = 1;
            // 
            // lblBaslik
            // 
            this.lblBaslik.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblBaslik.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblBaslik.Location = new System.Drawing.Point(16, 16);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Size = new System.Drawing.Size(300, 18);
            this.lblBaslik.TabIndex = 0;
            this.lblBaslik.Text = "Kitap Başlığı *";
            // 
            // txtBaslik
            // 
            this.txtBaslik.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.txtBaslik.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBaslik.ForeColor = System.Drawing.Color.White;
            this.txtBaslik.Location = new System.Drawing.Point(16, 40);
            this.txtBaslik.Name = "txtBaslik";
            this.txtBaslik.Size = new System.Drawing.Size(500, 28);
            this.txtBaslik.TabIndex = 1;
            // 
            // lblYazar
            // 
            this.lblYazar.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblYazar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblYazar.Location = new System.Drawing.Point(16, 84);
            this.lblYazar.Name = "lblYazar";
            this.lblYazar.Size = new System.Drawing.Size(232, 18);
            this.lblYazar.TabIndex = 2;
            this.lblYazar.Text = "Yazar";
            // 
            // txtYazar
            // 
            this.txtYazar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.txtYazar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYazar.ForeColor = System.Drawing.Color.White;
            this.txtYazar.Location = new System.Drawing.Point(16, 108);
            this.txtYazar.Name = "txtYazar";
            this.txtYazar.Size = new System.Drawing.Size(232, 28);
            this.txtYazar.TabIndex = 3;
            // 
            // lblKategori
            // 
            this.lblKategori.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblKategori.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblKategori.Location = new System.Drawing.Point(264, 84);
            this.lblKategori.Name = "lblKategori";
            this.lblKategori.Size = new System.Drawing.Size(252, 18);
            this.lblKategori.TabIndex = 4;
            this.lblKategori.Text = "Kategori";
            // 
            // cmbKategori
            // 
            this.cmbKategori.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.cmbKategori.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbKategori.ForeColor = System.Drawing.Color.White;
            this.cmbKategori.FormattingEnabled = true;
            this.cmbKategori.Location = new System.Drawing.Point(264, 108);
            this.cmbKategori.Name = "cmbKategori";
            this.cmbKategori.Size = new System.Drawing.Size(252, 25);
            this.cmbKategori.TabIndex = 5;
            this.cmbKategori.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            // 
            // lblYayinci
            // 
            this.lblYayinci.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblYayinci.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblYayinci.Location = new System.Drawing.Point(16, 152);
            this.lblYayinci.Name = "lblYayinci";
            this.lblYayinci.Size = new System.Drawing.Size(232, 18);
            this.lblYayinci.TabIndex = 6;
            this.lblYayinci.Text = "Yayıncı";
            // 
            // txtYayinci
            // 
            this.txtYayinci.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.txtYayinci.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYayinci.ForeColor = System.Drawing.Color.White;
            this.txtYayinci.Location = new System.Drawing.Point(16, 176);
            this.txtYayinci.Name = "txtYayinci";
            this.txtYayinci.Size = new System.Drawing.Size(232, 28);
            this.txtYayinci.TabIndex = 7;
            // 
            // lblYil
            // 
            this.lblYil.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblYil.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblYil.Location = new System.Drawing.Point(264, 152);
            this.lblYil.Name = "lblYil";
            this.lblYil.Size = new System.Drawing.Size(120, 18);
            this.lblYil.TabIndex = 8;
            this.lblYil.Text = "Yayın Yılı";
            // 
            // txtYil
            // 
            this.txtYil.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.txtYil.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYil.ForeColor = System.Drawing.Color.White;
            this.txtYil.Location = new System.Drawing.Point(264, 176);
            this.txtYil.Name = "txtYil";
            this.txtYil.Size = new System.Drawing.Size(120, 28);
            this.txtYil.TabIndex = 9;
            // 
            // lblISBN
            // 
            this.lblISBN.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblISBN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblISBN.Location = new System.Drawing.Point(16, 220);
            this.lblISBN.Name = "lblISBN";
            this.lblISBN.Size = new System.Drawing.Size(232, 18);
            this.lblISBN.TabIndex = 10;
            this.lblISBN.Text = "ISBN";
            // 
            // txtISBN
            // 
            this.txtISBN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.txtISBN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtISBN.ForeColor = System.Drawing.Color.White;
            this.txtISBN.Location = new System.Drawing.Point(16, 244);
            this.txtISBN.Name = "txtISBN";
            this.txtISBN.Size = new System.Drawing.Size(180, 28);
            this.txtISBN.TabIndex = 11;
            //
            // btnSearchIsbn
            //
            this.btnSearchIsbn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.btnSearchIsbn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchIsbn.FlatAppearance.BorderSize = 0;
            this.btnSearchIsbn.ForeColor = System.Drawing.Color.White;
            this.btnSearchIsbn.Location = new System.Drawing.Point(200, 243);
            this.btnSearchIsbn.Name = "btnSearchIsbn";
            this.btnSearchIsbn.Size = new System.Drawing.Size(48, 30);
            this.btnSearchIsbn.TabIndex = 12;
            this.btnSearchIsbn.Text = "Ara 🔍";
            this.btnSearchIsbn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchIsbn.Click += new System.EventHandler(this.BtnSearchIsbn_Click);
            // 
            // lblKopya
            // 
            this.lblKopya.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblKopya.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblKopya.Location = new System.Drawing.Point(264, 220);
            this.lblKopya.Name = "lblKopya";
            this.lblKopya.Size = new System.Drawing.Size(120, 18);
            this.lblKopya.TabIndex = 12;
            this.lblKopya.Text = "Kopya Sayısı";
            // 
            // txtKopya
            // 
            this.txtKopya.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.txtKopya.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKopya.ForeColor = System.Drawing.Color.White;
            this.txtKopya.Location = new System.Drawing.Point(264, 244);
            this.txtKopya.Name = "txtKopya";
            this.txtKopya.Size = new System.Drawing.Size(120, 28);
            this.txtKopya.TabIndex = 13;
            // 
            // lblAciklama
            // 
            this.lblAciklama.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblAciklama.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblAciklama.Location = new System.Drawing.Point(16, 288);
            this.lblAciklama.Name = "lblAciklama";
            this.lblAciklama.Size = new System.Drawing.Size(300, 18);
            this.lblAciklama.TabIndex = 14;
            this.lblAciklama.Text = "Açıklama";
            // 
            // txtAciklama
            // 
            this.txtAciklama.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.txtAciklama.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAciklama.ForeColor = System.Drawing.Color.White;
            this.txtAciklama.Location = new System.Drawing.Point(16, 312);
            this.txtAciklama.Multiline = true;
            this.txtAciklama.Name = "txtAciklama";
            this.txtAciklama.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAciklama.Size = new System.Drawing.Size(500, 80);
            this.txtAciklama.TabIndex = 15;
            // 
            // picCover
            // 
            this.picCover.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.picCover.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picCover.Location = new System.Drawing.Point(550, 40);
            this.picCover.Name = "picCover";
            this.picCover.Size = new System.Drawing.Size(200, 260);
            this.picCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCover.TabIndex = 20;
            this.picCover.TabStop = false;
            // 
            // btnSelectCover
            // 
            this.btnSelectCover.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(65)))));
            this.btnSelectCover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelectCover.FlatAppearance.BorderSize = 0;
            this.btnSelectCover.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectCover.ForeColor = System.Drawing.Color.White;
            this.btnSelectCover.Location = new System.Drawing.Point(550, 310);
            this.btnSelectCover.Name = "btnSelectCover";
            this.btnSelectCover.Size = new System.Drawing.Size(200, 30);
            this.btnSelectCover.TabIndex = 21;
            this.btnSelectCover.Text = "🖼️ Kapak Resmi Seç";
            this.btnSelectCover.UseVisualStyleBackColor = false;
            // 
            // lblPdfEkle
            // 
            this.lblPdfEkle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPdfEkle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblPdfEkle.Location = new System.Drawing.Point(550, 350);
            this.lblPdfEkle.Name = "lblPdfEkle";
            this.lblPdfEkle.Size = new System.Drawing.Size(200, 18);
            this.lblPdfEkle.TabIndex = 22;
            this.lblPdfEkle.Text = "Dijital Kopya (PDF/EPUB)";
            // 
            // txtPdfPath
            // 
            this.txtPdfPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(35)))));
            this.txtPdfPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPdfPath.ForeColor = System.Drawing.Color.DimGray;
            this.txtPdfPath.Location = new System.Drawing.Point(550, 370);
            this.txtPdfPath.Name = "txtPdfPath";
            this.txtPdfPath.ReadOnly = true;
            this.txtPdfPath.Size = new System.Drawing.Size(155, 28);
            this.txtPdfPath.TabIndex = 23;
            this.txtPdfPath.Text = "PDF veya EPUB seçilmedi...";
            // 
            // btnSelectPdf
            // 
            this.btnSelectPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(65)))));
            this.btnSelectPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelectPdf.FlatAppearance.BorderSize = 0;
            this.btnSelectPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectPdf.ForeColor = System.Drawing.Color.White;
            this.btnSelectPdf.Location = new System.Drawing.Point(710, 370);
            this.btnSelectPdf.Name = "btnSelectPdf";
            this.btnSelectPdf.Size = new System.Drawing.Size(40, 28);
            this.btnSelectPdf.TabIndex = 24;
            this.btnSelectPdf.Text = "...";
            this.btnSelectPdf.UseVisualStyleBackColor = false;
            // 
            // btnIptal
            // 
            this.btnIptal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(65)))));
            this.btnIptal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIptal.FlatAppearance.BorderSize = 0;
            this.btnIptal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIptal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnIptal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(185)))), ((int)(((byte)(215)))));
            this.btnIptal.Location = new System.Drawing.Point(16, 412);
            this.btnIptal.Name = "btnIptal";
            this.btnIptal.Size = new System.Drawing.Size(110, 40);
            this.btnIptal.TabIndex = 16;
            this.btnIptal.Text = "İptal";
            this.btnIptal.UseVisualStyleBackColor = false;
            this.btnIptal.Click += new System.EventHandler(this.BtnIptal_Click);
            // 
            // btnKaydet
            // 
            this.btnKaydet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.btnKaydet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnKaydet.FlatAppearance.BorderSize = 0;
            this.btnKaydet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKaydet.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnKaydet.ForeColor = System.Drawing.Color.White;
            this.btnKaydet.Location = new System.Drawing.Point(136, 412);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(614, 40);
            this.btnKaydet.TabIndex = 17;
            this.btnKaydet.Text = "➕  Kitabı Ekle";
            this.btnKaydet.UseVisualStyleBackColor = false;
            this.btnKaydet.Click += new System.EventHandler(this.BtnKaydet_Click);
            // 
            // AddBookForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(800, 561);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddBookForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Yeni Kitap Ekle";
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).EndInit();
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlAccent;
        public System.Windows.Forms.Label lblFormTitle;
        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.Label lblBaslik;
        private System.Windows.Forms.TextBox txtBaslik;
        private System.Windows.Forms.Label lblYazar;
        private System.Windows.Forms.TextBox txtYazar;
        private System.Windows.Forms.Label lblYayinci;
        private System.Windows.Forms.Label lblKategori;
        private System.Windows.Forms.ComboBox cmbKategori;
        private System.Windows.Forms.TextBox txtYayinci;
        private System.Windows.Forms.Label lblYil;
        private System.Windows.Forms.TextBox txtYil;
        private System.Windows.Forms.Label lblISBN;
        private System.Windows.Forms.TextBox txtISBN;
        private System.Windows.Forms.Label lblKopya;
        private System.Windows.Forms.TextBox txtKopya;
        private System.Windows.Forms.Label lblAciklama;
        private System.Windows.Forms.TextBox txtAciklama;
        private System.Windows.Forms.Button btnIptal;
        public System.Windows.Forms.Button btnKaydet;
        
        // Yeni Eklenen Araçlar
        private System.Windows.Forms.PictureBox picCover;
        private System.Windows.Forms.Button btnSelectCover;
        private System.Windows.Forms.Label lblPdfEkle;
        private System.Windows.Forms.TextBox txtPdfPath;
        private System.Windows.Forms.Button btnSelectPdf;
        private System.Windows.Forms.Button btnSearchIsbn;

    }
}
