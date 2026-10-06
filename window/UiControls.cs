using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BOM_compare.ui
{
    /// <summary>
    /// 统一配色，对应 Tauri 版 src/styles.css 中的 CSS 变量。
    /// </summary>
    internal static class UiTheme
    {
        public static readonly Color Primary = ColorTranslator.FromHtml("#2563EB");
        public static readonly Color PrimaryDark = ColorTranslator.FromHtml("#1D4ED8");
        public static readonly Color PrimaryLight = ColorTranslator.FromHtml("#EFF6FF");

        public static readonly Color Danger = ColorTranslator.FromHtml("#DC2626");
        public static readonly Color DangerLight = ColorTranslator.FromHtml("#FEF2F2");
        public static readonly Color DangerBorder = ColorTranslator.FromHtml("#FECACA");
        public static readonly Color DangerText = ColorTranslator.FromHtml("#B91C1C");

        public static readonly Color Warning = ColorTranslator.FromHtml("#D97706");
        public static readonly Color WarningLight = ColorTranslator.FromHtml("#FFFBEB");
        public static readonly Color WarningBorder = ColorTranslator.FromHtml("#FDE68A");

        public static readonly Color Success = ColorTranslator.FromHtml("#16A34A");
        public static readonly Color SuccessLight = ColorTranslator.FromHtml("#F0FDF4");
        public static readonly Color SuccessBorder = ColorTranslator.FromHtml("#BBF7D0");

        public static readonly Color Bg = ColorTranslator.FromHtml("#F1F5F9");
        public static readonly Color Card = Color.White;
        public static readonly Color Surface = ColorTranslator.FromHtml("#F8FAFC");
        public static readonly Color Border = ColorTranslator.FromHtml("#E2E8F0");
        public static readonly Color Text = ColorTranslator.FromHtml("#0F172A");
        public static readonly Color Muted = ColorTranslator.FromHtml("#64748B");
        public static readonly Color DisabledBg = ColorTranslator.FromHtml("#94A3B8");
        public static readonly Color ToastBg = ColorTranslator.FromHtml("#0F172A");

        public const string FontFamily = "Microsoft YaHei UI";

        /// <summary>构造圆角矩形路径</summary>
        public static GraphicsPath RoundedRect(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0f || rect.Width <= 0f || rect.Height <= 0f)
            {
                path.AddRectangle(rect);
                return path;
            }

            float d = Math.Min(radius * 2f, Math.Min(rect.Width, rect.Height));
            path.AddArc(rect.X, rect.Y, d, d, 180f, 90f);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270f, 90f);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0f, 90f);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90f, 90f);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>开启双缓冲的容器面板（减少滚动/重绘闪烁）</summary>
    internal class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }

    /// <summary>
    /// 圆角卡片面板：支持圆角半径、边框颜色/线宽/虚实线。
    /// </summary>
    internal class RoundedPanel : Panel
    {
        public int Radius { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }
        public DashStyle BorderDash { get; set; }

        public RoundedPanel()
        {
            Radius = 12;
            BorderColor = UiTheme.Border;
            BorderWidth = 1f;
            BorderDash = DashStyle.Solid;
            BackColor = UiTheme.Card;

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 先铺父容器背景，圆角外沿即可透出父级颜色
            Color canvas = Parent != null ? Parent.BackColor : UiTheme.Bg;
            using (SolidBrush bg = new SolidBrush(canvas))
            {
                g.FillRectangle(bg, ClientRectangle);
            }

            if (Width <= 0 || Height <= 0) return;

            float bw = BorderWidth;
            RectangleF rect = new RectangleF(bw / 2f, bw / 2f, Width - bw, Height - bw);
            using (GraphicsPath path = UiTheme.RoundedRect(rect, Radius))
            {
                using (SolidBrush fill = new SolidBrush(BackColor))
                {
                    g.FillPath(fill, path);
                }

                if (bw > 0f)
                {
                    using (Pen pen = new Pen(BorderColor, bw))
                    {
                        pen.DashStyle = BorderDash;
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }

    internal enum ButtonVariant
    {
        /// <summary>白底描边按钮（选择文件）</summary>
        Select,
        /// <summary>蓝色实心主按钮</summary>
        Primary,
        /// <summary>纯文字链接按钮</summary>
        Link
    }

    /// <summary>圆角按钮，对应 .btn / .btn-select / .btn-primary / .btn-link</summary>
    internal class RoundedButton : Button
    {
        private bool hovering;

        public int Radius { get; set; }
        public ButtonVariant Variant { get; set; }

        public RoundedButton()
        {
            Radius = 8;
            Variant = ButtonVariant.Select;

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Font = new Font(UiTheme.FontFamily, 10.5F, FontStyle.Bold);

            MouseEnter += delegate { hovering = true; Invalidate(); };
            MouseLeave += delegate { hovering = false; Invalidate(); };
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color canvas = Parent != null ? Parent.BackColor : UiTheme.Card;
            using (SolidBrush bg = new SolidBrush(canvas))
            {
                g.FillRectangle(bg, ClientRectangle);
            }

            Color fill;
            Color border;
            Color fore;

            if (Variant == ButtonVariant.Primary)
            {
                fill = !Enabled ? UiTheme.DisabledBg : (hovering ? UiTheme.PrimaryDark : UiTheme.Primary);
                border = fill;
                fore = Color.White;
            }
            else if (Variant == ButtonVariant.Link)
            {
                fill = Color.Empty;
                border = Color.Empty;
                fore = Enabled ? UiTheme.Primary : UiTheme.DisabledBg;
            }
            else
            {
                fill = hovering ? UiTheme.Surface : UiTheme.Card;
                border = hovering ? UiTheme.Primary : UiTheme.Border;
                fore = Enabled ? UiTheme.Text : UiTheme.DisabledBg;
            }

            RectangleF rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using (GraphicsPath path = UiTheme.RoundedRect(rect, Radius))
            {
                if (!fill.IsEmpty)
                {
                    using (SolidBrush b = new SolidBrush(fill)) g.FillPath(b, path);
                }
                if (!border.IsEmpty)
                {
                    using (Pen p = new Pen(border)) g.DrawPath(p, path);
                }
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (Variant == ButtonVariant.Link && hovering && Enabled)
            {
                Size ts = TextRenderer.MeasureText(Text, Font);
                int x = (Width - ts.Width) / 2;
                int y = (Height + ts.Height) / 2 - 2;
                using (Pen p = new Pen(fore)) g.DrawLine(p, x, y, x + ts.Width, y);
            }
        }
    }

    /// <summary>标签页按钮，对应 .tab / .tab.active（下划线指示当前页）</summary>
    internal class TabButton : Button
    {
        private bool hovering;
        private bool active;

        public bool Active
        {
            get { return active; }
            set
            {
                if (active == value) return;
                active = value;
                Invalidate();
            }
        }

        public TabButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            BackColor = UiTheme.Card;
            Font = new Font(UiTheme.FontFamily, 10.5F, FontStyle.Bold);

            MouseEnter += delegate { hovering = true; Invalidate(); };
            MouseLeave += delegate { hovering = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (SolidBrush b = new SolidBrush(BackColor))
            {
                g.FillRectangle(b, ClientRectangle);
            }

            Color fore = active ? UiTheme.Primary : (hovering ? UiTheme.Text : UiTheme.Muted);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            Color line = active ? UiTheme.Primary : UiTheme.Border;
            using (Pen p = new Pen(line, 2f))
            {
                g.DrawLine(p, 0f, Height - 1.5f, Width, Height - 1.5f);
            }
        }
    }
}
