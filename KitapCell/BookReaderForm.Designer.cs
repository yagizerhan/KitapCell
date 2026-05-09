using Microsoft.Web.WebView2.WinForms;

namespace KitapCell
{
    partial class BookReaderForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlTopBar    = new System.Windows.Forms.Panel();
            this.lblTitle     = new System.Windows.Forms.Label();
            this.lblPageInfo  = new System.Windows.Forms.Label();
            this.lblSessionInfo = new System.Windows.Forms.Label();
            this.btnMinimize  = new System.Windows.Forms.Button();
            this.btnMaximize  = new System.Windows.Forms.Button();
            this.btnClose     = new System.Windows.Forms.Button();
            this.webView      = new Microsoft.Web.WebView2.WinForms.WebView2();

            this.pnlTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize   = new System.Drawing.Size(1100, 820);
            this.MinimumSize  = new System.Drawing.Size(800, 600);
            this.Text         = "📖 PDF Okuyucu";
            this.BackColor    = System.Drawing.Color.FromArgb(13, 17, 23);
            this.ForeColor    = System.Drawing.Color.White;
            this.Font         = new System.Drawing.Font("Segoe UI", 9.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Padding      = new System.Windows.Forms.Padding(2);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.pnlTopBar);

            // ── pnlTopBar ─────────────────────────────────────────────────────
            this.pnlTopBar.BackColor    = System.Drawing.Color.FromArgb(22, 27, 34);
            this.pnlTopBar.Dock         = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Height       = 48;
            this.pnlTopBar.Padding      = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.pnlTopBar.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTitle, this.lblPageInfo, this.lblSessionInfo, this.btnMinimize, this.btnMaximize, this.btnClose
            });

            // lblTitle
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location  = new System.Drawing.Point(12, 10);
            this.lblTitle.Size      = new System.Drawing.Size(450, 28);
            this.lblTitle.AutoSize  = false;

            // lblPageInfo
            this.lblPageInfo.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPageInfo.ForeColor = System.Drawing.Color.FromArgb(99, 102, 241);
            this.lblPageInfo.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblPageInfo.Size      = new System.Drawing.Size(120, 28);
            this.lblPageInfo.Location  = new System.Drawing.Point(470, 10);
            this.lblPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPageInfo.Text      = "Sayfa: -";

            // lblSessionInfo
            this.lblSessionInfo.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSessionInfo.ForeColor = System.Drawing.Color.FromArgb(100, 107, 130);
            this.lblSessionInfo.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblSessionInfo.Size      = new System.Drawing.Size(300, 28);
            this.lblSessionInfo.Location  = new System.Drawing.Point(600, 10);
            this.lblSessionInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSessionInfo.Text      = "";

            // btnMinimize
            this.btnMinimize.Text            = "—";
            this.btnMinimize.Font            = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnMinimize.ForeColor       = System.Drawing.Color.White;
            this.btnMinimize.BackColor       = System.Drawing.Color.FromArgb(40, 45, 50);
            this.btnMinimize.FlatStyle       = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.Anchor          = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnMinimize.Size            = new System.Drawing.Size(40, 32);
            this.btnMinimize.Location        = new System.Drawing.Point(896, 8);
            this.btnMinimize.Cursor          = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.Click          += (s, e) => this.WindowState = System.Windows.Forms.FormWindowState.Minimized;

            // btnMaximize
            this.btnMaximize.Text            = "🗖";
            this.btnMaximize.Font            = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnMaximize.ForeColor       = System.Drawing.Color.White;
            this.btnMaximize.BackColor       = System.Drawing.Color.FromArgb(40, 45, 50);
            this.btnMaximize.FlatStyle       = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaximize.FlatAppearance.BorderSize = 0;
            this.btnMaximize.Anchor          = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnMaximize.Size            = new System.Drawing.Size(40, 32);
            this.btnMaximize.Location        = new System.Drawing.Point(942, 8);
            this.btnMaximize.Cursor          = System.Windows.Forms.Cursors.Hand;
            this.btnMaximize.Click          += (s, e) => {
                this.WindowState = this.WindowState == System.Windows.Forms.FormWindowState.Maximized 
                    ? System.Windows.Forms.FormWindowState.Normal 
                    : System.Windows.Forms.FormWindowState.Maximized;
            };

            // btnClose
            this.btnClose.Text            = "✕";
            this.btnClose.Font            = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor       = System.Drawing.Color.White;
            this.btnClose.BackColor       = System.Drawing.Color.FromArgb(60, 35, 35);
            this.btnClose.FlatStyle       = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Anchor          = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Size            = new System.Drawing.Size(40, 32);
            this.btnClose.Location        = new System.Drawing.Point(988, 8);
            this.btnClose.Cursor          = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Click          += (s, e) => this.Close();

            // Responsive: butonlar sağda sabit dursun
            this.Resize += (s, e) => {
                this.btnClose.Location = new System.Drawing.Point(this.ClientSize.Width - 52, 8);
                this.btnMaximize.Location = new System.Drawing.Point(this.ClientSize.Width - 98, 8);
                this.btnMinimize.Location = new System.Drawing.Point(this.ClientSize.Width - 144, 8);
                this.lblSessionInfo.Location = new System.Drawing.Point(this.ClientSize.Width - 460, 10);
            };

            // ── WebView2 ──────────────────────────────────────────────────────
            this.webView.Dock          = System.Windows.Forms.DockStyle.Fill;
            this.webView.BackColor     = System.Drawing.Color.FromArgb(13, 17, 23);

            // ── Resume ───────────────────────────────────────────────────────
            this.pnlTopBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel  pnlTopBar;
        private System.Windows.Forms.Label  lblTitle;
        private System.Windows.Forms.Label  lblPageInfo;
        private System.Windows.Forms.Label  lblSessionInfo;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Button btnClose;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
    }
}
