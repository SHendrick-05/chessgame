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
            Calcs.board = playBoard; // Set calc variables
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PictureBox box = (PictureBox)playBoard.GetControlFromPosition(i, j);
                    if (box != null)
                    {
                        ChessPiece piece = new ChessPiece(box, playBoard);
                        Calcs.pieces.Add(piece); // Set the "pieces" var in Calcs
                        if (piece.isWhite)
                            Calcs.wP.Add(piece);
                        else
                            Calcs.bP.Add(piece);
                    }
                }
            }
            Calcs.BK = Calcs.CheckPiece(bKe);
            Calcs.WK = Calcs.CheckPiece(wKe);
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
        //
        // Basic button controls
        //
        private void closeboard_Click(object sender, EventArgs e) 
        {
            Main mn = new Main();
            mn.Show();
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
                PictureBox boxC = (PictureBox)playBoard.GetControlFromPosition(col, i);
                PictureBox boxR = (PictureBox)playBoard.GetControlFromPosition(i, row);
                ChessPiece pcC = Calcs.CheckPiece(boxC);
                ChessPiece pcR = Calcs.CheckPiece(boxR);
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
        // Stalemate chec
        //
        private bool isSM(bool[] isCM)
        {
            return ((isCM[0] && whiteTurn)
                 || (isCM[1] && !whiteTurn));
        }

        //
        // Turn a move into notation
        //
        private string[] GetMoveText(bool attack, TableLayoutPanelCellPosition pos, ChessPiece piece, bool Check, bool NM, bool ep)
        {
            // Vars for checking
            string fS = "abcdefgh", rS = "87654321";
            Rank r = piece.pieceRank;
            bool P = r == Rank.PAWN; 

            // Rank/capture
            string pieceRank = !P ? r.ToString()[0].ToString() : ""; 
            string attC = attack ? "x" : "";
            string epS = ep ? " e.p." : "";

            // Positioning
            bool[] clear = isClear(piece);
            string Brank = !clear[1] ? rS[pos.Row].ToString() : "";
            string Bfile = !clear[0] || (P && attack) && P ? fS[pos.Column].ToString() : "";

            // Moveto positioning
            char rank = rS[piece.pos.Row];
            char file = fS[piece.pos.Column]; 

            // Checks/Checkmates/Stalemates
            bool CM = Check && NM;
            string check = !NM && Check ? "+" : "";
            string sCM = NM && Check ? "#" : "";
            string sSM = NM && !Check ? "$" : "";

            string W = whiteTurn && CM ? "0" : "1";
            string B = !whiteTurn && CM ? "0" : "1";

            string winLine = "";
            if (NM) winLine = Check ? W + "-" + B : "½-½";

            List<string> lines = new List<string>() { pieceRank+Bfile+Brank+attC+file+rank+check+sCM+epS, winLine };
            return lines.Where(i => i != "").ToArray(); // concaternate all of these chars

        }
        //
        // Piece moving
        //

        // Vars
        internal List<ChessPiece> checkingPieces = new List<ChessPiece>();
        internal ChessPiece selectedPiece;
        internal bool whiteTurn = true;
        internal bool isMoving = false;
        internal bool isCheck = false;
        internal bool isOver = false;

        
        
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
            isCheck = false;
            //
            // Get vars + Positions
            //
            PictureBox box = (PictureBox)sender;
            TableLayoutPanelCellPosition pos = playBoard.GetPositionFromControl(box);
            TableLayoutPanelCellPosition selpos = selectedPiece.pos;
            //
            // Passant check before move
            //
            bool ep = false;
            if (box.Name.Contains("DEL") 
                && selectedPiece.pieceRank == Rank.PAWN
                && pos.Column != selpos.Column)
            {
                ep = true;
                attack = true;
                playBoard.Controls.Remove
                    (playBoard.GetControlFromPosition
                    (pos.Column, selpos.Row));
            }
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
                selectedPiece.CheckPromote();
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
            List<ChessPiece> isCheckL = Calcs.CheckCheck(whiteTurn);
            checkingPieces = isCheckL;
            bool isCheckB = isCheckL.Count != 0;
            isCheck = isCheckB;
            //
            // Get CM
            //
            bool ifMoves = Calcs.NMCheck(whiteTurn, isCheckL);
            if (!ifMoves) 
            {
                // Game is over
                isOver = true;
                winScreen.Visible = true;
                turnbox.Visible = false;
                turntext.Visible = false;
                replay.Visible = true;

                // Find Checkmate/Stalemate
                if (isCheckB)
                    winText.Text = whiteTurn ? "BLACK WINS!" : "WHITE WINS!";
                else
                    winText.Text = "STALEMATE!";
            }

            // Update moves
            string[] lines = GetMoveText(attack, selpos, selectedPiece, isCheckB, !ifMoves, ep);
            List<string> boxLines = moves.Lines.ToList();
            if (boxLines.Count != 0)
            {
                string last = boxLines[boxLines.Count - 1];
                if (!last.Contains(' '))
                {
                    last += (' ' + lines[0]);
                    boxLines[boxLines.Count - 1] = last;
                }
                else
                    boxLines = boxLines.Concat(lines).ToList();
                moves.Lines = boxLines.ToArray();
            }
            else
                moves.Lines = lines;
            
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
            if (isOver) return;
            PictureBox box = (PictureBox)sender;
            ChessPiece selpiece = Calcs.CheckPiece(box);
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
            List<Point> moves = Calcs.CalcMovesG(selpiece, checkingPieces);
            List<Point> discard = new List<Point>();
            foreach (Point pt in moves)
            {
                int col = pt.X;
                int row = pt.Y;
                PictureBox cont = (PictureBox)playBoard.GetControlFromPosition(pt.X, pt.Y);
                ChessPiece contp = Calcs.CheckPiece(cont);
                //
                // Discard unusable moves
                //
                if ( col < 0 || col > 7 || row < 0 || row > 7 )
                    discard.Add(pt);
                else if ( cont != null
                    && cont.BackColor != Color.DarkGray && contp.isWhite == selpiece.isWhite )
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

        private void replay_Click(object sender, EventArgs e)
        {
            Board brd = new Board();
            brd.Show();
            Close();
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
 * 
 * 
 * 
 * 
            /*
            bool ifMoves = Calcs.NMCheck(whiteTurn, isCheckT[
            if (isCheckB)
            {
                bool[] isCM = Calcs.NMCheck(isCheckT);
                if (isCM[0] || isCM[1])
                {
                    if (isCM[2]) // Checkmate
                    {
                        isOver = true;
                        winText.Text = isCM[0] ? "BLACK WINS!" : "WHITE WINS!";
                        winScreen.Visible = true;
                        replay.Visible = true;
                        turnbox.Visible = false;
                        turntext.Visible = false;
                    }
                    else // (Possible) Stalemate
                    {

                        {
                            isOver = true;
                            winText.Text = "STALEMATE!";
                            winScreen.Visible = true;
                            replay.Visible = true;
                            turnbox.Visible = false;
                            turntext.Visible = false;
                        }
                    }
                }
            }
              */
