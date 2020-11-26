using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System;
using Chess.Properties;

namespace Chess
{
    internal class ChessPiece
    {

        //
        // Variables
        //
        internal static Dictionary<char, Bitmap[]> Images = new Dictionary<char, Bitmap[]>()
        {
            {'R', new Bitmap[2] {Resources.wrook, Resources.brook}},
            {'K', new Bitmap[2] {Resources.wknight, Resources.bknight}},
            {'B', new Bitmap[2] {Resources.wbishop, Resources.bbishop}},
            {'Q', new Bitmap[2] {Resources.wqueen, Resources.bqueen}}
        };

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
        internal void PromotePawn(char rank)
        {
            if (!"RQBK".Contains(rank.ToString())) return;
            box.Name = box.Name[0].ToString() + rank + box.Name[2].ToString();
            box.BackgroundImage = Images[rank][Convert.ToInt32(isWhite)];
        }
        internal bool CheckPromote()
        {
            if (pieceRank != Rank.PAWN)
                return false;
            if (isWhite && pos.Row == 0)
                return true;
            else if (!isWhite && pos.Row == 7)
                return true;
            return false;
        }
        internal bool canDouble = true;
        internal bool PassElig = false;
    }


}