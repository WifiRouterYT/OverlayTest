using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        //private const int LWA_ALPHA = 0x2;
        //private const int LWA_COLORKEY = 0x1;

        public Form1()
        {
            init();
        }

        private void init()
        {
            this.Size = new Size(SystemInformation.PrimaryMonitorSize.Width,
                               SystemInformation.PrimaryMonitorSize.Height);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(1, 1, 1); // match backColor and TransparencyKey
            this.TransparencyKey = Color.FromArgb(1, 1, 1);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.DoubleBuffer, true);

            MakeClickThrough();
        }

        private void MakeClickThrough()
        {
            var extendedStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawRoundedBox(g, 20, 20, 300, 80, 15, Color.Black, "hi chat", Color.White);
        }


        private void DrawRoundedBox(Graphics g, int x, int y, int width, int height, int radius,
                                   Color backgroundColor, string text, Color textColor)
        {
            GraphicsPath path = new GraphicsPath();
            float inset = 0.5f;
            float fx = x + inset;
            float fy = y + inset;
            float fwidth = width - (inset * 2);
            float fheight = height - (inset * 2);

            path.AddArc(fx, fy, radius * 2, radius * 2, 180, 90);
            path.AddArc(fx + fwidth - radius * 2, fy, radius * 2, radius * 2, 270, 90);
            path.AddArc(fx + fwidth - radius * 2, fy + fheight - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(fx, fy + fheight - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseAllFigures();

            using (SolidBrush brush = new SolidBrush(backgroundColor))
            {
                g.FillPath(brush, path);
            }

            using (Font font = new Font("Segoe UI", 12, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;

                Rectangle textRect = new Rectangle(x + 15, y + 10, width - 30, height - 20);
                g.DrawString(text, font, textBrush, textRect, sf);
            }

            path.Dispose();
        }



        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT;
                return cp;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
