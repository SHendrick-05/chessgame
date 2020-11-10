using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Chess
{
    internal enum Rank
    {
        PAWN,
        ROOK,
        NIGH,
        BISH,
        QUEE,
        KING
    }
    internal enum Dir
    {
        F,
        L,
        B,
        R
    }
    internal static class Calcs
    {
        //
        // Piece handling
        //
        static internal List<ChessPiece> pieces = new List<ChessPiece>(); // Global piece list [used by all]
        static internal ChessPiece WK;
        static internal ChessPiece BK;

        static internal ChessPiece CheckPiece(PictureBox checkbox, TableLayoutPanel board)
        {
            if (checkbox == null)
                return null;
            foreach (ChessPiece piece in pieces)
            {
                if (piece.box == checkbox)
                    return piece;
            }
            ChessPiece newpiece = new ChessPiece(checkbox, board);
            pieces.Add(newpiece);
            return newpiece;
        }
        //
        // Check check
        //
        static internal List<ChessPiece>[] CheckCheck(TableLayoutPanel board) // [White,Black], if team is in check
        {
            List<ChessPiece> WC = new List<ChessPiece>();
            List<ChessPiece> BC = new List<ChessPiece>();
            if (WK == null || BK == null)
                throw new Exception("One or more kings are missing");
            Point WP = new Point(WK.pos.Column, WK.pos.Row);
            Point BP = new Point(BK.pos.Column, BK.pos.Row);
            foreach (ChessPiece pc in pieces)
            {
                if (pc.box.Name.Contains("DEL")) continue;
                List<Point> moves = CalcMovesG(pc, board);
                if (moves.Contains(WP) && !pc.isWhite)
                    WC.Add(pc);
                if (moves.Contains(BP) && pc.isWhite)
                    BC.Add(pc);
            }
            return new List<ChessPiece>[2] { WC, BC };

        }
        //
        // CM Check
        //
        static internal bool[] CMCheck(TableLayoutPanel board, List<ChessPiece>[] Checks) // [White,black], returns if checkmate.
        {
            List<Point> WM = new List<Point>(), BM = new List<Point>();
            foreach (ChessPiece pc in pieces)
            {
                if (pc.isWhite)
                    foreach (Point pt in CalcMovesG(pc, board, Checks[0]))
                    {
                        WM.Add(pt);
                    }
                else
                    foreach (Point pt in CalcMovesG(pc, board, Checks[1]))
                    {
                        BM.Add(pt);
                    }
            }
            return new bool[2] { WM.Count == 0, BM.Count == 0 };

        }
        //[[
        // Move calcs
        //]]

        //  These next few functions are general, and a loop for lines/diag
        // Function for the loop
        //
        static private int[] LoopFunc(int[] xy, List<Dir> direction)
        {
            int x = xy[0];
            int y = xy[1];

            foreach (Dir drct in direction)
            {
                switch (drct)
                {
                    case Dir.F:
                        y--;
                        break;
                    case Dir.R:
                        x++;
                        break;
                    case Dir.B:
                        y++;
                        break;
                    case Dir.L:
                        x--;
                        break;
                }
            }
            return new int[2] { x, y };
        }
        //
        // Check for the loop
        //
        static private bool CheckVals(params int[] vals)
        {
            foreach (int x in vals)
            {
                if (x < 0 || x > 7)
                    return false;
            }
            return true;
        }
        //
        // Loop to calculate Diag/Lines
        //
        static private List<Point> LoopCalc(ChessPiece piece, TableLayoutPanel board, List<Dir> direction, int distance = 10)
        {
            List<Point> result = new List<Point>();
            TableLayoutPanelCellPosition pos = board.GetPositionFromControl(piece.box);
            int row = pos.Row;
            int col = pos.Column;
            int counter = 0;
            for (int[] xy = new int[2] { col, row }; CheckVals(xy); xy = LoopFunc(xy, direction))
            {
                if (distance == counter++) break;
                Point pt = new Point(xy[0], xy[1]);
                result.Add(pt);
                PictureBox box = (PictureBox)board.GetControlFromPosition(xy[0], xy[1]);
                if (box == null)
                    result.Add(pt);
                else
                {
                    if (box.Name == piece.box.Name) continue;
                    if (CheckPiece(box, board).isWhite != piece.isWhite
                        && box.BackColor != Color.DarkGray)
                        result.Add(pt);
                    break;
                }

            }
            return result;
        }
        //
        // Intermediate function: Calculates lines
        //
        static private List<Point> CalcLines(ChessPiece piece, TableLayoutPanel board, int distance = 10)
        {
            List<Point> resF = LoopCalc(piece, board, new List<Dir>() { Dir.F }, distance);
            List<Point> resR = LoopCalc(piece, board, new List<Dir>() { Dir.R }, distance);
            List<Point> resB = LoopCalc(piece, board, new List<Dir>() { Dir.B }, distance);
            List<Point> resL = LoopCalc(piece, board, new List<Dir>() { Dir.L }, distance);
            return resF
                .Concat(resR)
                .Concat(resB)
                .Concat(resL)
                .ToList();
        }
        //
        // Intermediate function: Calculate diag
        //
        static private List<Point> CalcDiag(ChessPiece piece, TableLayoutPanel board, int distance = 10)
        {
            List<Point> resFR = LoopCalc(piece, board, new List<Dir>() { Dir.F, Dir.R }, distance);
            List<Point> resBR = LoopCalc(piece, board, new List<Dir>() { Dir.B, Dir.R }, distance);
            List<Point> resBL = LoopCalc(piece, board, new List<Dir>() { Dir.B, Dir.L }, distance);
            List<Point> resFL = LoopCalc(piece, board, new List<Dir>() { Dir.F, Dir.L }, distance);
            return resFR
                .Concat(resBR)
                .Concat(resBL)
                .Concat(resFL)
                .ToList();
        }
        //
        // The next functions are rank-specific
        //
        //
        // Pawn
        //
        static private List<Point> CalcPawn(ChessPiece piece, TableLayoutPanel board)
        {
            //
            // Vars
            //
            List<Point> result = new List<Point>();
            int offset = piece.isWhite ? -1 : 1;
            TableLayoutPanelCellPosition pos = board.GetPositionFromControl(piece.box);
            int row = pos.Row;
            int col = pos.Column;
            //
            // Straight move 
            //
            if ((PictureBox)board.GetControlFromPosition(col, row + offset) == null)
                result.Add(new Point(col, row + offset));
            if (piece.canDouble && (PictureBox)board.GetControlFromPosition(col, row + (2 * offset)) == null)
                result.Add(new Point(col, row + (2 * offset)));

            for (int x = 1; x >= -1; x -= 2)
            {
                if (!CheckVals(x + col))
                    continue;
                //
                // Diag attacks
                //
                PictureBox pbox = (PictureBox)board.GetControlFromPosition(col + x, row + offset);
                if (pbox != null)
                {
                    ChessPiece ppiece = CheckPiece(pbox, board);
                    if (ppiece.isWhite != piece.isWhite)
                        result.Add(new Point(col + x, row + offset));
                }
                //
                // En Passant
                //
                pbox = (PictureBox)board.GetControlFromPosition(col + x, row);
                if (pbox != null)
                {
                    ChessPiece ppiece = CheckPiece(pbox, board);
                    if ((ppiece.isWhite != piece.isWhite) && ppiece.PassElig)
                        result.Add(new Point(col + x, row + offset));
                }
            }

            return result;
        }
        //
        // Rook
        //
        static private List<Point> CalcRook(ChessPiece piece, TableLayoutPanel board)
        {
            return CalcLines(piece, board);
        }
        //
        // Knight
        //
        static private List<Point> CalcKnight(ChessPiece piece, TableLayoutPanel board)
        {

            TableLayoutPanelCellPosition pos = board.GetPositionFromControl(piece.box);
            int y = pos.Row;
            int x = pos.Column;
            List<Point> result = new List<Point>()
                { // Calculate each point individually, TODO: optimise
                    new Point(x+1,y+2),
                    new Point(x-1,y+2),
                    new Point(x+1,y-2),
                    new Point(x-1,y-2),
                    new Point(x+2,y+1),
                    new Point(x-2,y+1),
                    new Point(x+2,y-1),
                    new Point(x-2,y-1)
                };
            List<Point> discard = new List<Point>();
            foreach (Point pt in result)
            {
                if (!CheckVals(pt.X, pt.Y))
                    discard.Add(pt);
            }
            foreach (Point pt in discard)
            {
                result.Remove(pt);
            }
            return result;
        }
        //
        // Bishop
        //
        static private List<Point> CalcBishop(ChessPiece piece, TableLayoutPanel board)
        {
            return CalcDiag(piece, board); // Diagonal extending to end of board
        }
        //
        // Queen
        //
        static private List<Point> CalcQueen(ChessPiece piece, TableLayoutPanel board)
        {
            List<Point> str = CalcLines(piece, board); // Line to end of board
            List<Point> diag = CalcDiag(piece, board); // Diag to end of board
            return str.Concat(diag)
                .ToList(); // Merge lists
        }
        //
        // King
        //
        static private List<Point> CalcKing(ChessPiece piece, TableLayoutPanel board)
        {
            List<Point> str = CalcLines(piece, board, 2); // Line 1 long
            List<Point> diag = CalcDiag(piece, board, 2); // Diag 1 long
            return str.Concat(diag)
                .ToList(); // Merge lists
        }
        //
        // Public calc interface
        //
        static public List<Point> CalcMovesG(ChessPiece piece, TableLayoutPanel board)
        {
            List<Point> moves;
            switch (piece.pieceRank)
            { // Access rank-specific methods
                case Rank.PAWN:
                    moves = CalcPawn(piece, board);
                    break;
                case Rank.ROOK:
                    moves = CalcRook(piece, board);
                    break;
                case Rank.NIGH:
                    moves = CalcKnight(piece, board);
                    break;
                case Rank.BISH:
                    moves = CalcBishop(piece, board);
                    break;
                case Rank.QUEE:
                    moves = CalcQueen(piece, board);
                    break;
                case Rank.KING:
                    moves = CalcKing(piece, board);
                    break;
                default:
                    moves = new List<Point>();
                    break;
            }
            return moves;

        }

        static private void Tbox(TableLayoutPanel board, Point pt)
        {
            PictureBox box = new PictureBox();
            box.Name = string.Format("TEMP_{0}{1}", pt.X.ToString(), pt.Y.ToString());
            box.BackColor = Color.DarkGray;
            board.Controls.Add(box, pt.X, pt.Y);
            box.Dock = DockStyle.Fill;
            box.Margin = new Padding(1);
        }

        
        static public List<Point> CalcMovesG(ChessPiece piece, TableLayoutPanel board, List<ChessPiece> checkingPieces)
        {
            List<Point> movesTemp = CalcMovesG(piece, board);
            if (checkingPieces.Count == 0) return movesTemp;
            List<Point> moves2 = new List<Point>();
            bool iW = piece.isWhite;
            List<Point> otherMoves = new List<Point>();
            foreach (ChessPiece pc in pieces)
            {
                if (pc.isWhite != iW)
                    otherMoves.Concat(CalcMovesG(piece, board));
            }

            foreach (Point pt in movesTemp)
            {
                if (piece.pieceRank == Rank.KING && !otherMoves.Contains(pt)) // King dodge
                    moves2.Add(pt);
                if (checkingPieces.Count == 1 && checkingPieces[0].posPT == pt) // Take the piece
                    moves2.Add(pt);
                if (board.GetControlFromPosition(pt.X, pt.Y) == null)
                {
                    Tbox(board, pt);
                    if (CheckCheck(board).All(i => i.Count == 0))
                        moves2.Add(pt);
                }// Block the move
            }
            return moves2;
        }

    }
}
    //
    // Piece class
    //



/*
 * 
 * Old code:
 
    internal class Pawn : ChessPiece
    {
        internal Pawn(PictureBox Box) : base(Box) { }
   
        internal bool canDouble;
        internal bool canPassLeft;
        internal bool canPassRight;
        internal List<TableLayoutPanelCellPosition> CalcMoves(PictureBox pbox, TableLayoutPanel board)
        {
            List<TableLayoutPanelCellPosition> result = new List<TableLayoutPanelCellPosition>();
            TableLayoutPanelCellPosition pos = board.GetPositionFromControl(pbox);
            int row = pos.Row;
            int col = pos.Column;


            throw new NotImplementedException();
        }
    }
    internal class Rook : ChessPiece
    {
        internal Rook(PictureBox Box) : base(Box) { }

    }
    internal class Knight : ChessPiece
    {
        internal Knight(PictureBox Box) : base(Box) { }

    }
    internal class Bishop : ChessPiece
    {
        internal Bishop(PictureBox Box) : base(Box) { }

    }
    internal class Queen : ChessPiece
    {
        internal Queen(PictureBox Box) : base(Box) { }

    }
        internal class King : ChessPiece
    {
        internal King(PictureBox Box) : base(Box) { }

    }
 
 * 
 *             List<Point> WM = pieces.Where(p => p.isWhite)
                .Select(piece => CalcMovesG(piece, board, Checks))
                .SelectMany(x => x)
                .ToList();
            List<Point> BM = pieces.Where(p => !p.isWhite)
                .Select(piece => CalcMovesG(piece, board, Checks))
                .SelectMany(x => x)
                .ToList();
 */
