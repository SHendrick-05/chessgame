
/*
 * Created by Sean Hendrick, 10P1
 * 
 * Basic guide to this program:
 * 
 *
 * Point to Position conversion:
 * X = col
 * Y = row
 * 
 * Enum guide:
 * 
 *  enum Rank:
 *      PAWN = pawn
 *      ROOK = rook
 *      KNIG = knight
 *      BISH = bishop
 *      QUEE = queen
 *      KING = king
 * 
 *  enum Dir:
 *      F = Forward ( White -> Black )
 *      B = Back    ( Black -> White )
 *      L = Left    ( Right -> Left  )
 *      R = Right   ( Left  -> Right )
 *      
 *  Dir to Point/Pos:
 *      F = Row--/y--
 *      B = Row++/y++
 *      L = Col--/x--
 *      R = Col++/x++
 */