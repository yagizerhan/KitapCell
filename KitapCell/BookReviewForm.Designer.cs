namespace KitapCell
{
    partial class BookReviewForm
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
            this.lblBookTitle = new System.Windows.Forms.Label();
            this.lblRating = new System.Windows.Forms.Label();
            this.pnlStars = new System.Windows.Forms.FlowLayoutPanel();
            this.lblReview = new System.Windows.Forms.Label();
            this.txtReview = new System.Windows.Forms.RichTextBox();
            this.btnKaydet = new System.Windows.Forms.Button();
            
            this.SuspendLayout();

            this.SetupStarButton(out this.btnStar1, "1");
            this.SetupStarButton(out this.btnStar2, "2");
            this.SetupStarButton(out this.btnStar3, "3");
            this.SetupStarButton(out this.btnStar4, "4");
            this.SetupStarButton(out this.btnStar5, "5");

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Text = "Kitap Değerlendirme";

            this.lblBookTitle.AutoSize = true;
            this.lblBookTitle.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblBookTitle.ForeColor = System.Drawing.Color.FromArgb(180, 185, 215);
            this.lblBookTitle.Location = new System.Drawing.Point(20, 55);
            this.lblBookTitle.Text = "...";

            this.lblRating.AutoSize = true;
            this.lblRating.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRating.ForeColor = System.Drawing.Color.White;
            this.lblRating.Location = new System.Drawing.Point(20, 100);
            this.lblRating.Text = "Puanınız:";

            this.pnlStars.Location = new System.Drawing.Point(24, 125);
            this.pnlStars.Size = new System.Drawing.Size(250, 44);
            this.pnlStars.Controls.AddRange(new System.Windows.Forms.Control[] { btnStar1, btnStar2, btnStar3, btnStar4, btnStar5 });
            
            this.lblReview.AutoSize = true;
            this.lblReview.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblReview.ForeColor = System.Drawing.Color.White;
            this.lblReview.Location = new System.Drawing.Point(20, 180);
            this.lblReview.Text = "Yorum/İncelemeniz:";

            this.txtReview.Location = new System.Drawing.Point(24, 205);
            this.txtReview.Size = new System.Drawing.Size(430, 100);
            this.txtReview.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtReview.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.txtReview.ForeColor = System.Drawing.Color.White;
            this.txtReview.BorderStyle = System.Windows.Forms.BorderStyle.None;

            this.btnKaydet.Location = new System.Drawing.Point(24, 330);
            this.btnKaydet.Size = new System.Drawing.Size(120, 40);
            this.btnKaydet.BackColor = System.Drawing.Color.FromArgb(34, 197, 94);
            this.btnKaydet.ForeColor = System.Drawing.Color.White;
            this.btnKaydet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKaydet.FlatAppearance.BorderSize = 0;
            this.btnKaydet.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnKaydet.Text = "Kaydet";
            this.btnKaydet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);

            this.ClientSize = new System.Drawing.Size(480, 400);
            this.BackColor = System.Drawing.Color.FromArgb(13, 17, 23);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Değerlendirme";

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblBookTitle);
            this.Controls.Add(this.lblRating);
            this.Controls.Add(this.pnlStars);
            this.Controls.Add(this.btnKaydet);

            this.Load += new System.EventHandler(this.BookReviewForm_Load);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupStarButton(out System.Windows.Forms.Button btn, string tag)
        {
            btn = new System.Windows.Forms.Button();
            btn.Size = new System.Drawing.Size(40, 40);
            btn.Tag = tag;
            btn.Font = new System.Drawing.Font("Segoe UI Emoji", 16F);
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            btn.BackColor = System.Drawing.Color.Transparent;
            btn.ForeColor = System.Drawing.Color.FromArgb(245, 158, 11); // Amber rcolor
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
            btn.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            btn.Click += new System.EventHandler(this.btnStar_Click);
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBookTitle;
        private System.Windows.Forms.Label lblRating;
        private System.Windows.Forms.FlowLayoutPanel pnlStars;
        private System.Windows.Forms.Button btnStar1;
        private System.Windows.Forms.Button btnStar2;
        private System.Windows.Forms.Button btnStar3;
        private System.Windows.Forms.Button btnStar4;
        private System.Windows.Forms.Button btnStar5;
        private System.Windows.Forms.Label lblReview;
        private System.Windows.Forms.RichTextBox txtReview;
        private System.Windows.Forms.Button btnKaydet;
    }
}
