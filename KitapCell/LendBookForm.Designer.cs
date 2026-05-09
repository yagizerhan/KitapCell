namespace KitapCell
{
    partial class LendBookForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSeciliKitap = new System.Windows.Forms.Label();
            this.lblUyeSec = new System.Windows.Forms.Label();
            this.cmbUsers = new System.Windows.Forms.ComboBox();
            this.lblTarih = new System.Windows.Forms.Label();
            this.dtpReturnDate = new System.Windows.Forms.DateTimePicker();
            this.btnOnayla = new System.Windows.Forms.Button();
            this.btnIptal = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Kitap Ödünç Verme";
            // 
            // lblSeciliKitap
            // 
            this.lblSeciliKitap.AutoSize = true;
            this.lblSeciliKitap.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSeciliKitap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(209)))), ((int)(((byte)(217)))));
            this.lblSeciliKitap.Location = new System.Drawing.Point(22, 70);
            this.lblSeciliKitap.Name = "lblSeciliKitap";
            this.lblSeciliKitap.Size = new System.Drawing.Size(100, 20);
            this.lblSeciliKitap.TabIndex = 1;
            this.lblSeciliKitap.Text = "Seçili Kitap: -";
            // 
            // lblUyeSec
            // 
            this.lblUyeSec.AutoSize = true;
            this.lblUyeSec.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblUyeSec.ForeColor = System.Drawing.Color.White;
            this.lblUyeSec.Location = new System.Drawing.Point(22, 110);
            this.lblUyeSec.Name = "lblUyeSec";
            this.lblUyeSec.Size = new System.Drawing.Size(73, 20);
            this.lblUyeSec.TabIndex = 2;
            this.lblUyeSec.Text = "Üye Seçin:";
            // 
            // cmbUsers
            // 
            this.cmbUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsers.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbUsers.FormattingEnabled = true;
            this.cmbUsers.Location = new System.Drawing.Point(26, 135);
            this.cmbUsers.Name = "cmbUsers";
            this.cmbUsers.Size = new System.Drawing.Size(300, 28);
            this.cmbUsers.TabIndex = 3;
            // 
            // lblTarih
            // 
            this.lblTarih.AutoSize = true;
            this.lblTarih.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTarih.ForeColor = System.Drawing.Color.White;
            this.lblTarih.Location = new System.Drawing.Point(22, 180);
            this.lblTarih.Name = "lblTarih";
            this.lblTarih.Size = new System.Drawing.Size(115, 20);
            this.lblTarih.TabIndex = 4;
            this.lblTarih.Text = "Son İade Tarihi:";
            // 
            // dtpReturnDate
            // 
            this.dtpReturnDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dtpReturnDate.Location = new System.Drawing.Point(26, 205);
            this.dtpReturnDate.Name = "dtpReturnDate";
            this.dtpReturnDate.Size = new System.Drawing.Size(300, 27);
            this.dtpReturnDate.TabIndex = 5;
            // 
            // btnOnayla
            // 
            this.btnOnayla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(160)))), ((int)(((byte)(67)))));
            this.btnOnayla.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOnayla.FlatAppearance.BorderSize = 0;
            this.btnOnayla.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOnayla.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnOnayla.ForeColor = System.Drawing.Color.White;
            this.btnOnayla.Location = new System.Drawing.Point(176, 260);
            this.btnOnayla.Name = "btnOnayla";
            this.btnOnayla.Size = new System.Drawing.Size(150, 40);
            this.btnOnayla.TabIndex = 6;
            this.btnOnayla.Text = "Ödünç Ver";
            this.btnOnayla.UseVisualStyleBackColor = false;
            this.btnOnayla.Click += new System.EventHandler(this.btnOnayla_Click);
            // 
            // btnIptal
            // 
            this.btnIptal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(70)))));
            this.btnIptal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIptal.FlatAppearance.BorderSize = 0;
            this.btnIptal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIptal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnIptal.ForeColor = System.Drawing.Color.White;
            this.btnIptal.Location = new System.Drawing.Point(26, 260);
            this.btnIptal.Name = "btnIptal";
            this.btnIptal.Size = new System.Drawing.Size(130, 40);
            this.btnIptal.TabIndex = 7;
            this.btnIptal.Text = "İptal";
            this.btnIptal.UseVisualStyleBackColor = false;
            this.btnIptal.Click += new System.EventHandler(this.btnIptal_Click);
            // 
            // LendBookForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(27)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(364, 331);
            this.Controls.Add(this.btnIptal);
            this.Controls.Add(this.btnOnayla);
            this.Controls.Add(this.dtpReturnDate);
            this.Controls.Add(this.lblTarih);
            this.Controls.Add(this.cmbUsers);
            this.Controls.Add(this.lblUyeSec);
            this.Controls.Add(this.lblSeciliKitap);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LendBookForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ödünç İşlemi";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSeciliKitap;
        private System.Windows.Forms.Label lblUyeSec;
        private System.Windows.Forms.ComboBox cmbUsers;
        private System.Windows.Forms.Label lblTarih;
        private System.Windows.Forms.DateTimePicker dtpReturnDate;
        private System.Windows.Forms.Button btnOnayla;
        private System.Windows.Forms.Button btnIptal;
    }
}
