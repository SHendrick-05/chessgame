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
using System.Media;

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

        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        public static void openMidi(String fileName, String alias)
        {
            mciSendString("open " + fileName + " type sequencer alias " + alias, new StringBuilder(), 0, new IntPtr());
            mciSendString("play " + alias, new StringBuilder(), 0, new IntPtr());
        }
        public static void playMidi(String alias)
        {
            
            mciSendString("resume " + alias, new StringBuilder(), 0, new IntPtr());
        }

        public static void pauseMidi(String alias)
        {
            mciSendString("pause " + alias, null, 0, new IntPtr());
        }

        public bool musicPlaying = true;

        //
        // Init
        //
        public Main()
        {
            InitializeComponent();
            openMidi(@"c:\Windows\media\onestop.mid", "Onestop");
            playMidi("Onestop");
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
            Environment.Exit(0);
        }

        private void source_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SHendrick-Turton/chessgame");
        }

        private void music_Click(object sender, EventArgs e)
        {
            if (musicPlaying)
            {
                pauseMidi("Onestop");
                music.Text = "Music on";
            }
            else
            {
                playMidi("Onestop");
                music.Text = "Music off";
            }
            musicPlaying = !musicPlaying;
        }
    }
}
