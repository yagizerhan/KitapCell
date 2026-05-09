namespace KitapCell
{
    partial class ReturnBookForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle    = new System.Windows.Forms.Label();
            this.lblBilgi    = new System.Windows.Forms.Label();
            this.dgvLoans    = new System.Windows.Forms.DataGridView();
            this.colId       = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKitap    = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUye      = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlis     = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSon      = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDurum    = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSecili   = new System.Windows.Forms.Label();
            this.btnIadeEt   = new System.Windows.Forms.Button();
            this.btnKapat    = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoans)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Text      = "📥 İade Al";
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location  = new System.Drawing.Point(16, 14);
            this.lblTitle.Size      = new System.Drawing.Size(300, 32);

            // lblBilgi
            this.lblBilgi.Text      = "Aktif ödünç kayıtları yükleniyor...";
            this.lblBilgi.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBilgi.ForeColor = System.Drawing.Color.FromArgb(139, 148, 158);
            this.lblBilgi.Location  = new System.Drawing.Point(16, 50);
            this.lblBilgi.Size      = new System.Drawing.Size(560, 20);

            // dgvLoans
            this.dgvLoans.BackgroundColor        = System.Drawing.Color.FromArgb(22, 27, 34);
            this.dgvLoans.BorderStyle            = System.Windows.Forms.BorderStyle.None;
            this.dgvLoans.GridColor              = System.Drawing.Color.FromArgb(48, 54, 61);
            this.dgvLoans.RowHeadersVisible      = false;
            this.dgvLoans.AllowUserToAddRows     = false;
            this.dgvLoans.AllowUserToDeleteRows  = false;
            this.dgvLoans.ReadOnly               = true;
            this.dgvLoans.SelectionMode          = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLoans.MultiSelect            = false;
            this.dgvLoans.RowTemplate.Height     = 38;
            this.dgvLoans.ColumnHeadersHeight    = 36;
            this.dgvLoans.EnableHeadersVisualStyles = false;
            this.dgvLoans.ColumnHeadersDefaultCellStyle.BackColor  = System.Drawing.Color.FromArgb(33, 38, 45);
            this.dgvLoans.ColumnHeadersDefaultCellStyle.ForeColor  = System.Drawing.Color.FromArgb(139, 148, 158);
            this.dgvLoans.ColumnHeadersDefaultCellStyle.Font       = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvLoans.ColumnHeadersBorderStyle                 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvLoans.DefaultCellStyle.BackColor               = System.Drawing.Color.FromArgb(13, 17, 23);
            this.dgvLoans.DefaultCellStyle.ForeColor               = System.Drawing.Color.FromArgb(201, 209, 217);
            this.dgvLoans.DefaultCellStyle.Font                    = new System.Drawing.Font("Segoe UI", 10F);
            this.dgvLoans.DefaultCellStyle.SelectionBackColor      = System.Drawing.Color.FromArgb(45, 51, 59);
            this.dgvLoans.DefaultCellStyle.SelectionForeColor      = System.Drawing.Color.White;
            this.dgvLoans.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.dgvLoans.Location                 = new System.Drawing.Point(16, 76);
            this.dgvLoans.Size                     = new System.Drawing.Size(756, 300);
            this.dgvLoans.AutoSizeColumnsMode      = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLoans.SelectionChanged        += new System.EventHandler(this.DgvLoans_SelectionChanged);

            // Columns
            this.colId.Name    = "colId";    this.colId.HeaderText    = "ID";       this.colId.FillWeight    = 30;  this.colId.Visible = false;
            this.colKitap.Name = "colKitap"; this.colKitap.HeaderText = "KİTAP";   this.colKitap.FillWeight = 200;
            this.colUye.Name   = "colUye";   this.colUye.HeaderText   = "ÜYE";     this.colUye.FillWeight   = 140;
            this.colAlis.Name  = "colAlis";  this.colAlis.HeaderText  = "ALIŞ";    this.colAlis.FillWeight   = 90;
            this.colSon.Name   = "colSon";   this.colSon.HeaderText   = "SON İADE"; this.colSon.FillWeight   = 90;
            this.colDurum.Name = "colDurum"; this.colDurum.HeaderText = "DURUM";   this.colDurum.FillWeight  = 90;
            this.dgvLoans.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colId, this.colKitap, this.colUye, this.colAlis, this.colSon, this.colDurum
            });

            // lblSecili
            this.lblSecili.Text      = "Bir kayıt seçin...";
            this.lblSecili.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.lblSecili.ForeColor = System.Drawing.Color.FromArgb(99, 102, 241);
            this.lblSecili.Location  = new System.Drawing.Point(16, 386);
            this.lblSecili.Size      = new System.Drawing.Size(580, 22);

            // btnIadeEt
            this.btnIadeEt.Text             = "✅ İade Et";
            this.btnIadeEt.Font             = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnIadeEt.BackColor        = System.Drawing.Color.FromArgb(46, 160, 67);
            this.btnIadeEt.ForeColor        = System.Drawing.Color.White;
            this.btnIadeEt.FlatStyle        = System.Windows.Forms.FlatStyle.Flat;
            this.btnIadeEt.FlatAppearance.BorderSize = 0;
            this.btnIadeEt.Location         = new System.Drawing.Point(602, 380);
            this.btnIadeEt.Size             = new System.Drawing.Size(170, 38);
            this.btnIadeEt.Enabled          = false;
            this.btnIadeEt.Cursor           = System.Windows.Forms.Cursors.Hand;
            this.btnIadeEt.Click           += new System.EventHandler(this.btnIadeEt_Click);

            // btnKapat
            this.btnKapat.Text             = "Kapat";
            this.btnKapat.Font             = new System.Drawing.Font("Segoe UI", 10F);
            this.btnKapat.BackColor        = System.Drawing.Color.FromArgb(50, 55, 70);
            this.btnKapat.ForeColor        = System.Drawing.Color.White;
            this.btnKapat.FlatStyle        = System.Windows.Forms.FlatStyle.Flat;
            this.btnKapat.FlatAppearance.BorderSize = 0;
            this.btnKapat.Location         = new System.Drawing.Point(16, 380);
            this.btnKapat.Size             = new System.Drawing.Size(100, 38);
            this.btnKapat.Cursor           = System.Windows.Forms.Cursors.Hand;
            this.btnKapat.Click           += new System.EventHandler(this.btnKapat_Click);

            // ReturnBookForm
            this.BackColor      = System.Drawing.Color.FromArgb(13, 17, 23);
            this.ClientSize     = new System.Drawing.Size(790, 436);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTitle, this.lblBilgi, this.dgvLoans, this.lblSecili, this.btnIadeEt, this.btnKapat
            });
            this.Font            = new System.Drawing.Font("Segoe UI", 9.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.Text            = "İade Al - Aktif Ödünçler";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoans)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label       lblTitle;
        private System.Windows.Forms.Label       lblBilgi;
        private System.Windows.Forms.DataGridView dgvLoans;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKitap;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUye;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlis;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSon;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDurum;
        private System.Windows.Forms.Label       lblSecili;
        private System.Windows.Forms.Button      btnIadeEt;
        private System.Windows.Forms.Button      btnKapat;
    }
}
