using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System;

namespace Chess
{
    internal class ChessPiece
    {
        //
        // Variables
        //
        internal PictureBox box { get; set; }
        internal TableLayoutPanel board;
        internal ChessPiece(PictureBox Box, TableLayoutPanel Board)
        {
            box = Box;
            board = Board;
        }
        internal bool isWhite { get 
        {
            return box.Name[0] == 'w';
        } }
        internal TableLayoutPanelCellPosition pos
        {
            get
            {
                return board.GetPositionFromControl(box);
            }
        }
        internal Point posPT
        {
            get
            {
                return new Point(pos.Column, pos.Row);
            }
        }
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
                        return Rank.NIGH;
                    case 'B':
                        return Rank.BISH;
                    case 'Q':
                        return Rank.QUEE;
                    case 'K':
                        return Rank.KING;
                    case 'E':
                        return Rank.PAWN;
                    default:
                        throw new Exception("ISSUE: INVALID RANKING");
                }
            }

        }
        internal bool canCollide = true;
        //
        // Pawn-specific functions
        //
        private void PromotePawn()
        {
            box.Name = string.Format("{0}Q{1}P", box.Name[0], box.Name[2]);
            box.BackgroundImage = isWhite ? Properties.Resources.wqueen : Properties.Resources.bqueen;
        }
        internal void CheckPromote()
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