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
                        ChessPiece piece = new ChessPiece(box, playBoard);
                        Calcs.pieces.Add(piece);
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
        // Basic check
        //
        private bool[] isClear(ChessPiece piece) // Returns [rank, file], [Col, row]
        {
            bool colB = true, rowB = true;
            int col = piece.pos.Column, row = piece.pos.Row;
            for (int i = 0; i < 8; i++)
            {
                PictureBox boxC = (PictureBox)playBoard.GetControlFromPosition(row, i);
                PictureBox boxR = (PictureBox)playBoard.GetControlFromPosition(i, col);
                ChessPiece pcC = Calcs.CheckPiece(boxC, playBoard);
                ChessPiece pcR = Calcs.CheckPiece(boxR,playBoard);
                if (boxC != null && pcC.pieceRank == piece.pieceRank
                    && pcC.isWhite == piece.isWhite
                    && pcC != piece)
                {
                    colB = false;
                }
                if (boxR != null && pcR.pieceRank == piece.pieceRank
                    && pcR.isWhite == piece.isWhite
                    && pcR != piece)
                {
                    rowB = false;
                }
            }
            return new bool[2] { colB, rowB };
        }

        //
        // Get move
        //


        private string[] GetMoveText(bool attack, TableLayoutPanelCellPosition pos, ChessPiece piece, bool Check)
        {
            string rS = "abcdefgh", fS = "87654321";
            Rank r = piece.pieceRank;
            bool P = r != Rank.PAWN;
            string pieceRank = P ? r.ToString()[0].ToString() : "";
            string attC = attack ? "x" : "";
            bool[] clear = isClear(piece);
            string Brank = !clear[0] && (P || attack) ? rS[pos.Column].ToString() : "";
            string Bfile = !clear[1] && P ? fS[pos.Row].ToString() : "";
            char rank = rS[piece.pos.Column];
            char file = fS[piece.pos.Row];
            string check = Check ? "+" : "";
            return new string[1] { pieceRank+Brank+Bfile+attC+rank+file+check };

        }
        //
        // Piece moving
        //

        // Vars
        internal ChessPiece selectedPiece;
        internal bool whiteTurn = true;
        internal bool isMoving = false;
        internal bool isCheck = false;
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
        // Clear temp boxes
        //
        internal void ClearTempBoxes()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PictureBox tbox = (PictureBox)playBoard.GetControlFromPosition(i, j);
                    if (tbox != null)
                    {
                        if (tbox.Name.Contains("DEL"))
                            playBoard.Controls.Remove(tbox);
                        tbox.BackColor = Color.Transparent;
                    }
                }
            }
        }
        //
        // Moving for attacking pieces
        //
        internal void AttackMove(object sender, MouseEventArgs e)
        {
            PictureBox box = (PictureBox)sender;
            if (selectedPiece.box == box)
            {
                selectedPiece = null;
                isMoving = false;
                ClearTempBoxes();
            }
            if (selectedPiece != null
                && box.BackColor == Color.DarkGray)
            {
                MovePiece(sender, e, true);
            }
            else return;
        }
        //
        // Main move function
        //
        internal void MovePiece(object sender, MouseEventArgs e, bool attack=false)
        {
            //
            // Get vars + Positions
            //
            PictureBox box = (PictureBox)sender;
            TableLayoutPanelCellPosition pos = playBoard.GetPositionFromControl(box);
            TableLayoutPanelCellPosition selpos = selectedPiece.pos;
            //
            // Passant check before move
            //
            if (box.Name.Contains("DEL") && selectedPiece.pieceRank == Rank.PAWN)
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
            foreach (ChessPiece pc in Calcs.pieces)
            {
                if (pc.isWhite == selectedPiece.isWhite)
                    pc.PassElig = false;
            }
            if (selectedPiece.pieceRank == Rank.PAWN)
            {
                if (Math.Abs(selpos.Row - pos.Row) == 2)
                    selectedPiece.PassElig = true;
                selectedPiece.CheckPromote(playBoard);
                selectedPiece.canDouble = false;
            }
            //
            // Remove temp boxes
            //
            ClearTempBoxes();
            //
            // Change turn text
            //
            whiteTurn = !whiteTurn;
            if (whiteTurn)
                turnbox.Text = "WHITE";
            else
                turnbox.Text = "BLACK";
            isMoving = false;
            //
            // Get check
            //
            bool[] isCheck = Calcs.CheckCheck(playBoard);
            //
            // Update moves
            //
            moves.Lines = moves.Lines.Concat(GetMoveText(attack,selpos,selectedPiece, isCheck.Contains(true) )).ToArray();
            //
            // Clear selected piece
            //
            selectedPiece = null;
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
            ChessPiece selpiece = Calcs.CheckPiece(box, playBoard);
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
            List<Point> moves = Calcs.CalcMovesG(selpiece, playBoard);
            List<Point> discard = new List<Point>();
            foreach (Point pt in moves)
            {
                int col = pt.X;
                int row = pt.Y;
                PictureBox cont = (PictureBox)playBoard.GetControlFromPosition(pt.X, pt.Y);
                ChessPiece contp = Calcs.CheckPiece(cont, playBoard);
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