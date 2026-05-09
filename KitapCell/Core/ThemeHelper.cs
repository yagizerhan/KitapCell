using System.Drawing;
using System.Windows.Forms;

namespace KitapCell.Core
{
    /// <summary>
    /// Tüm dialog formlarına tutarlı koyu tema uygular.
    /// </summary>
    public static class ThemeHelper
    {
        // Renk paleti — MainForm ile aynı
        public static readonly Color BgDeep    = Color.FromArgb( 13,  17,  23);  // #0d1117
        public static readonly Color BgBase    = Color.FromArgb( 22,  27,  34);  // #161b22
        public static readonly Color BgPanel   = Color.FromArgb( 33,  38,  45);  // #21262d
        public static readonly Color BorderClr = Color.FromArgb( 48,  54,  61);  // #30363d
        public static readonly Color TxtMain   = Color.FromArgb(201, 209, 217);  // #c9d1d9
        public static readonly Color TxtMuted  = Color.FromArgb(139, 148, 158);  // #8b949e
        public static readonly Color Accent    = Color.FromArgb( 99, 102, 241);  // indigo

        /// <summary>
        /// Forma ve içindeki tüm kontrollerine koyu temayı uygular.
        /// </summary>
        public static void Apply(Form form)
        {
            form.BackColor  = BgDeep;
            form.ForeColor  = TxtMain;
            form.Font       = new Font("Segoe UI", 10F);
            // Başlık çubuğunu koyu yapmak için DwmAPI (Windows 11 / 10 destekli)
            TryDarkTitleBar(form);
            ApplyToControls(form.Controls);
        }

        private static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                switch (ctrl)
                {
                    case DataGridView dgv:
                        StyleDgv(dgv);
                        break;
                    case Panel p:
                        if (p.BackColor == SystemColors.Control || p.BackColor == Color.White)
                            p.BackColor = BgBase;
                        p.ForeColor = TxtMain;
                        ApplyToControls(p.Controls);
                        break;
                    case Label lbl:
                        if (lbl.ForeColor == SystemColors.ControlText)
                            lbl.ForeColor = TxtMain;
                        if (lbl.BackColor == SystemColors.Control)
                            lbl.BackColor = Color.Transparent;
                        break;
                    case TextBox txt:
                        txt.BackColor = BgPanel;
                        txt.ForeColor = TxtMain;
                        txt.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case RichTextBox rtb:
                        rtb.BackColor = BgPanel;
                        rtb.ForeColor = TxtMain;
                        rtb.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case ComboBox cmb:
                        cmb.BackColor = BgPanel;
                        cmb.ForeColor = TxtMain;
                        cmb.FlatStyle = FlatStyle.Flat;
                        break;
                    case ListBox lb:
                        lb.BackColor = BgPanel;
                        lb.ForeColor = TxtMain;
                        lb.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case CheckBox chk:
                        if (chk.ForeColor == SystemColors.ControlText)
                            chk.ForeColor = TxtMain;
                        if (chk.BackColor == SystemColors.Control)
                            chk.BackColor = Color.Transparent;
                        break;
                    case DateTimePicker dtp:
                        dtp.BackColor = BgPanel;
                        dtp.ForeColor = TxtMain;
                        dtp.CalendarMonthBackground = BgBase;
                        dtp.CalendarForeColor = TxtMain;
                        dtp.CalendarTitleBackColor = BgPanel;
                        dtp.CalendarTitleForeColor = Accent;
                        break;
                    case NumericUpDown nud:
                        nud.BackColor = BgPanel;
                        nud.ForeColor = TxtMain;
                        break;
                    case TabControl tab:
                        tab.Appearance = TabAppearance.Normal;
                        tab.DrawMode = TabDrawMode.OwnerDrawFixed;
                        tab.DrawItem += (s, e) =>
                        {
                            var tp = tab.TabPages[e.Index];
                            var fillColor = e.Index == tab.SelectedIndex ? BgBase : BgDeep;
                            using var brush = new SolidBrush(fillColor);
                            e.Graphics.FillRectangle(brush, e.Bounds);
                            TextRenderer.DrawText(e.Graphics, tp.Text, tab.Font,
                                e.Bounds, TxtMain, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        };
                        foreach (TabPage tp in tab.TabPages)
                        {
                            tp.BackColor = BgBase;
                            tp.ForeColor = TxtMain;
                            ApplyToControls(tp.Controls);
                        }
                        break;
                    case PictureBox:
                        ctrl.BackColor = BgPanel;
                        break;
                    default:
                        ApplyToControls(ctrl.Controls);
                        break;
                }
            }
        }

        private static void StyleDgv(DataGridView dgv)
        {
            dgv.BackgroundColor          = BgDeep;
            dgv.GridColor                = BorderClr;
            dgv.BorderStyle              = BorderStyle.None;
            dgv.RowHeadersVisible        = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AllowUserToAddRows       = false;
            dgv.SelectionMode            = DataGridViewSelectionMode.FullRowSelect;

            dgv.DefaultCellStyle.BackColor          = BgDeep;
            dgv.DefaultCellStyle.ForeColor          = TxtMain;
            dgv.DefaultCellStyle.SelectionBackColor = BgPanel;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font               = new Font("Segoe UI", 10F);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = BgBase;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TxtMain;

            dgv.ColumnHeadersDefaultCellStyle.BackColor  = BgPanel;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor  = TxtMuted;
            dgv.ColumnHeadersDefaultCellStyle.Font       = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersBorderStyle                 = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight                      = 36;
            dgv.RowTemplate.Height                       = 36;
        }

        /// <summary>
        /// Windows 10/11'de başlık çubuğunu koyu yapar.
        /// </summary>
        private static void TryDarkTitleBar(Form form)
        {
            try
            {
                // DWMWA_USE_IMMERSIVE_DARK_MODE = 20 (Windows 11), 19 (Windows 10)
                var hwnd = form.Handle;
                int attrValue = 1;
                NativeMethods.DwmSetWindowAttribute(hwnd, 20, ref attrValue, sizeof(int));
            }
            catch { /* Eski Windows'ta desteklenmez, sessizce atla */ }
        }
    }

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        internal static extern int DwmSetWindowAttribute(
            nint hwnd, int attr, ref int attrValue, int attrSize);
    }
}
