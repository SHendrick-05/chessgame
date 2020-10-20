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
        KNIG,
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
    internal static class MoveCalcs
    {
        //
        // Piece handling
        //
        static internal List<ChessPiece> pieces = new List<ChessPiece>(); // Global piece list [used by all]

        static internal ChessPiece CheckPiece(PictureBox checkbox)
        {
            if (checkbox == null)
                return null;
            foreach (ChessPiece piece in pieces)
            {
                if (piece.box == checkbox)
                    return piece;
            }
            ChessPiece newpiece = new ChessPiece(checkbox);
            pieces.Add(newpiece);
            return newpiece;
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

            foreach(Dir drct in direction)
            {
                switch(drct)
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
            return new int[2] {x, y};
        }
        //
        // Check for the loop
        //
        static private bool LoopCheck(int[] xy)
        {
            return (xy[0] >= 0 && xy[0] <= 7) && (xy[1] >= 0 && xy[1] <= 7);
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
            for (int[] xy = new int[2] { col, row }; LoopCheck(xy); xy = LoopFunc(xy, direction))
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
                    if (CheckPiece(box).isWhite != piece.isWhite)
                        result.Add(pt);
                    break;
                }
                
            }
            return result;
        }
        //
        // Intermediate function: Calculates lines
        //
        static private List<Point> CalcLines(ChessPiece piece, TableLayoutPanel board, int distance=10)
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
        static private List<Point> CalcDiag(ChessPiece piece, TableLayoutPanel board, int distance=10)
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
            if ((PictureBox)board.GetControlFromPosition(col, row + offset) == null )
                result.Add(new Point(col, row + offset));
            if (piece.canDouble && (PictureBox)board.GetControlFromPosition(col, row + (2 * offset)) == null)
                result.Add(new Point(col, row + (2 * offset))); 

            for (int x = 1; x >= -1; x -= 2)
            {
                if ( (x == -1 && col == 0)
                    || (x == 1 && col == 7) )
                    continue;
                //
                // Diag attacks
                //
                PictureBox pbox = (PictureBox)board.GetControlFromPosition(col + x, row + offset); 
                if (pbox != null)
                {
                   ChessPiece ppiece = CheckPiece(pbox);
                   if (ppiece.isWhite != piece.isWhite)
                       result.Add(new Point(col + x, row + offset));
                }
                //
                // En Passant
                //
                pbox = (PictureBox)board.GetControlFromPosition(col + x, row);
                if (pbox != null)
                {
                    ChessPiece ppiece = CheckPiece(pbox);
                    if ((ppiece.isWhite != piece.isWhite) && ppiece.PassElig)
                        result.Add(new Point(col+x, row+offset));
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
                    new Point(x-2,y-2)
                };
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
            List<Point> str = CalcLines(piece, board, 1); // Line 1 long
            List<Point> diag = CalcDiag(piece, board, 1); // Diag 1 long
            return str.Concat(diag)
                .ToList(); // Merge lists
        }
        //
        // Public calc interface
        //
        static public List<Point> CalcMovesG(ChessPiece piece, TableLayoutPanel board)
        {
            switch (piece.pieceRank)
            { // Access rank-specific methods
                case Rank.PAWN:
                    return CalcPawn(piece, board);
                case Rank.ROOK:
                    return CalcRook(piece, board);
                case Rank.KNIG:
                    return CalcKnight(piece, board);
                case Rank.BISH:
                    return CalcBishop(piece, board);
                case Rank.QUEE:
                    return CalcQueen(piece, board);
                default:
                    return CalcKing(piece, board);
            }
        }
    }
    //
    // Piece class
    //
    internal class ChessPiece
    {
        //
        // Variables
        //
        internal PictureBox box { get; set; }
        internal ChessPiece(PictureBox Box)
        {
            box = Box;
        }
        internal bool isWhite { get 
        {
            return box.Name[0] == 'w';
        } }
        internal Rank pieceRank
        {
            get
            {
                switch (box.Name[1])
                {
                    case 'P':
                        return Rank.PAWN;
                    case 'R':
                        return Rank.ROOK;
                    case 'N':
                        return Rank.KNIG;
                    case 'B':
                        return Rank.BISH;
                    case 'Q':
                        return Rank.QUEE;
                    default:
                        return Rank.KING;
                }
            }

        }
        //
        // General funcs
        //
        internal List<Point> CalcMoves(TableLayoutPanel board)
        {
            return MoveCalcs.CalcMovesG(this, board);
        }
        //
        // Pawn-specific functions
        //
        private void PromotePawn()
        {
            box.Name = string.Format("{0}Q{1}P", box.Name[0], box.Name[2]);
            box.BackgroundImage = isWhite ? Properties.Resources.wqueen : Properties.Resources.bqueen;
        }
        internal void CheckPromote(TableLayoutPanel board)
        {
            if (pieceRank != 0)
                return;
            TableLayoutPanelCellPosition boxpos = board.GetPositionFromControl(box);
            if (isWhite && boxpos.Row == 0)
                PromotePawn();
            else if (!isWhite && boxpos.Row == 7)
                PromotePawn();
        }
        internal bool canDouble = true;
        internal bool PassElig = false;
    }


}



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
 
 
 */
