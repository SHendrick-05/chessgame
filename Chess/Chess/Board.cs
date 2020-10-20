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
using System.IO;
using System.Reflection;

namespace Chess
{
    public partial class Board : Form
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
        public Board()
        {
            InitializeComponent();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PictureBox box = (PictureBox)playBoard.GetControlFromPosition(i, j);
                    if (box != null)
                    {
                        ChessPiece piece = new ChessPiece(box);
                        MoveCalcs.pieces.Add(piece);
                    }
                }
            }
        }
        //
        // Colouring
        //
        private void colourBoard(object sender, TableLayoutCellPaintEventArgs e)
        {
            if ((e.Column + e.Row) % 2 == 1)
                e.Graphics.FillRectangle(Brushes.DarkOliveGreen, e.CellBounds);
            else
                e.Graphics.FillRectangle(Brushes.PapayaWhip, e.CellBounds); 
        }

        private void rank_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
        }

        private void file_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
        }
        //
        // Basic button controls
        //
        private void closeboard_Click(object sender, EventArgs e) 
        {
            Close();
        }
        //
        // Piece moving
        //

        // Vars
        internal ChessPiece selectedPiece;
        internal bool whiteTurn = true;
        internal bool isMoving;

        //
        // Boxes to indicate available moves
        //
        internal void AddTempBox(int row, int col)
        {
            PictureBox currBox = (PictureBox)playBoard.GetControlFromPosition(col, row);
            if (currBox == null)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Name = string.Format("DEL_{0}{1}", col.ToString(), row.ToString());
                pictureBox.BackColor = Color.DarkGray;
                playBoard.Controls.Add(pictureBox, col, row);
                pictureBox.Dock = DockStyle.Fill;
                pictureBox.Margin = new Padding(1);
                pictureBox.MouseClick += new MouseEventHandler(TempPieceClick);
            }
            else
            {
                currBox.BackColor = Color.DarkGray;
            }
        }
        //
        // Click event for box
        //
        internal void TempPieceClick(object sender, MouseEventArgs e)
        {
            PictureBox box = (PictureBox)sender;
            if (box.Name.Contains("DEL"))
            {
                MovePiece(sender, e);
                return;
            }
        }
        //
        // Moving, (Attack is a check)
        //
        internal void AttackMove(object sender, MouseEventArgs e)
        {
            PictureBox box = (PictureBox)sender;
            if (selectedPiece != null
                && box.BackColor == Color.Blue)
            {
                MovePiece(sender, e);
            }
            else return;
        }
        //
        // Main move function
        //
        internal void MovePiece(object sender, MouseEventArgs e)
        {
            if (selectedPiece != null)
            {
                //
                // Get vars + Positions
                //
                PictureBox box = (PictureBox)sender;
                TableLayoutPanelCellPosition pos = playBoard.GetPositionFromControl(box);
                TableLayoutPanelCellPosition selpos = playBoard.GetPositionFromControl(selectedPiece.box);
                //
                // Passant check before move
                //
                if (box.Name.Contains("DEL"))
                    playBoard.Controls.Remove
                        (playBoard.GetControlFromPosition
                        (pos.Column, selpos.Row));
                //
                // Move the piece.
                //
                playBoard.Controls.Remove(box);
                playBoard.Controls.Add(selectedPiece.box, pos.Column, pos.Row);
                //
                // Pawn checks
                //
                foreach (ChessPiece pc in MoveCalcs.pieces)
                {
                    if (pc.isWhite == selectedPiece.isWhite)
                        pc.PassElig = false;
                }
                if (Math.Abs(selpos.Row - pos.Row) == 2)
                    selectedPiece.PassElig = true;
                Console.WriteLine(selectedPiece.PassElig);
                selectedPiece.CheckPromote(playBoard);
                selectedPiece.canDouble = false;
                //
                // Clear selected piece
                //
                selectedPiece = null;
                
            }
            //
            // Remove temp boxes
            //
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PictureBox tbox = (PictureBox)playBoard.GetControlFromPosition(i, j);
                    if (tbox != null)
                    {
                        tbox.BackColor = Color.Transparent;
                        if (tbox.Name.Contains("DEL"))
                            playBoard.Controls.Remove(tbox);
                    }
                }
            }
            //
            // Change turn text
            //
            if (whiteTurn)
                turnbox.Text = "WHITE";
            else
                turnbox.Text = "BLACK";
            isMoving = false;
        }
        //
        // Handle piece clicks
        //
        internal void PieceClick(object sender, MouseEventArgs e)
        {
            //
            // Discard moves/wrong clicks
            //
            PictureBox box = (PictureBox)sender;
            ChessPiece selpiece = MoveCalcs.CheckPiece(box);
            if (isMoving)
            {
                AttackMove(sender, e);
                return;
            }
            else if (whiteTurn != selpiece.isWhite) return;
            isMoving = true;
            
            //
            // Get moves + Calculate
            // Variables
            selectedPiece = selpiece;
            List<Point> moves = MoveCalcs.CalcMovesG(selpiece, playBoard);
            List<Point> discard = new List<Point>();
            foreach (Point pt in moves)
            {
                int col = pt.X;
                int row = pt.Y;
                PictureBox cont = (PictureBox)playBoard.GetControlFromPosition(pt.X, pt.Y);
                ChessPiece contp = MoveCalcs.CheckPiece(cont);
                //
                // Discard unusable moves
                //
                if ( col < 0 || col > 7 || row < 0 || row > 7 )
                    discard.Add(pt);
                else if ( cont != null && contp.isWhite == selpiece.isWhite )
                    discard.Add(pt);
                else
                    AddTempBox(row, col);
            }
            if (moves.Count == discard.Count)
            {
                isMoving = false;
                return;
            }
            whiteTurn = !whiteTurn;
        }
           
        //
        //
        //
    }
}





/*
 * 
 * Old code:
 * 
 *  
            string name = box.Name;
            bool isWhite = name[0] == 'w';
            int pieceRank = 0;
            switch (name[1])
            {
                case 'P':
                    pieceRank = 0;
                    break;
                case 'R':
                    pieceRank = 1;
                    break;
                case 'N':
                    pieceRank = 2;
                    break;
                case 'B':
                    pieceRank = 3;
                    break;
                case 'Q':
                    pieceRank = 4;
                    break;
                case 'K':
                    pieceRank = 5;
                    break;
            }
            foreach(TableLayoutPanelCellPosition pos in CalcMoves(pieceRank, box))
            {
                int column = pos.Column;
                int row = pos.Row;

                PictureBox cont = (PictureBox)playBoard.GetControlFromPosition(column, row);
                if (cont == null
                    || (cont != null && (cont.Name[0] == 'w') != isWhite))
                {
                    AddTempBox(row, column);
                }
            }
  
 * 
 * 
 * 
 *          internal List<TableLayoutPanelCellPosition> CalcAttac(int piece, PictureBox box, TableLayoutPanelCellPosition pos)
        {
            List<TableLayoutPanelCellPosition> result = new List<TableLayoutPanelCellPosition>();
            int pRow = pos.Row + (box.Name[0] == 'w' ? -1 : 1);
            int pCol = pos.Column + 1;
            switch (piece)
            {
                case 0:
                    PictureBox tBox = (PictureBox)playBoard.GetControlFromPosition(pCol, pRow);
                    if (tBox != null)
                    {
                        tBox.BackColor = Color.Blue;
                    }
                    pCol -= 2;
                    tBox = (PictureBox)playBoard.GetControlFromPosition(pCol, pRow);
                    if (tBox != null)
                    {
                        tBox.BackColor = Color.Blue;
                    }
                    break;
            }
            return result;
        }
*/