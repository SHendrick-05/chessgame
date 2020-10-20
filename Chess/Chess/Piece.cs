using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    class Piece
    {
        public PictureBox box;
        public string name
        {
            get
            {
                return box.Name;
            }
        }
        public bool isWhite
        {
            get
            {
                return name[0] == 'w' ? true : false;
            }
        }
        public string pieceval { get; set; }
        public int rank { get; set; }
        public int file { get; set; }

        public Piece(PictureBox play)
        {
            box = play;
            Main pl = new Main();
            pl.Text = name;
            pl.ShowDialog();
        }
    }
}
