using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class Main : Form
    {
        //
        // Draggable top
        //
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void Drag(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        //
        // Init
        //
        public Main()
        {
            InitializeComponent();
        }
        //
        // Button funcs
        //
        private void Play(object sender, EventArgs e)
        {
            Board brd = new Board();
            brd.Show();
            Hide();
        }

        private void CloseApp(object sender, EventArgs e)
        {
            Close();
        }

        private void source_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SHendrick-Turton/chessgame");
        }
    }
}
