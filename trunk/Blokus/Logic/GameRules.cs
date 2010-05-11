using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    public class GameRules
    {
        public const int VioletStartPositionX = 4;
        public const int VioletStartPositionY = 4;

        public const int OrangeStartPositionX = 9;
        public const int OrangeStartPositionY = 9;

        private const int EmptyHandBonus = 15;
        private const int MonominoLastBonus = 5;

        public static List<Move> GetMoves(GameState gameState)
        {
            return GetMoves(gameState, int.MaxValue);
        }

        public static List<Move> GetMoves(GameState gameState, int moveCount)
        {           
            var boardExt = CreateBoard(gameState);

            bool isGameNew = SetStartPositions(gameState, boardExt);

            FillCorners(gameState, boardExt);
         
            var corners = CreateBoardCornersList(boardExt);

            var result = GetMovesList(gameState, boardExt, corners, moveCount, isGameNew);

            return result;
        }

        private static bool SetStartPositions(GameState gameState, BoardCell[,] board)
        {
            if (gameState.CurrentPlayerColor == Player.Orange)
            {
                if (board[OrangeStartPositionX, OrangeStartPositionY] == BoardCell.Empty)
                {
                    board[OrangeStartPositionX, OrangeStartPositionY] = BoardCell.Corner;
                    return true;
                }
            }
            else if (gameState.CurrentPlayerColor == Player.Violet)
            {
                if (board[VioletStartPositionX, VioletStartPositionY] == BoardCell.Empty)
                {
                    board[VioletStartPositionX, VioletStartPositionY] = BoardCell.Corner;
                    return true;
                }
            }
            return false;
        }

        private static List<Move> GetMovesList(
            GameState gameState, 
            BoardCell[,] boardExt, 
            List<PiecePosition> corners, 
            int moveCount,
            bool isGameNew)
        {
            var result = new List<Move>();
            if (moveCount <= 0)
            {
                return result;
            }

            foreach (var piece in gameState.CurrentPlayerHand.HandPieces)
            {
                for (int variant = 0; variant < piece.Variants.Length; variant++)
                {
                    var pieceCornerIndicies = isGameNew ? 
                        piece.Variants[variant].AllIndicies : 
                        piece.Variants[variant].CornersIndicies;

                    foreach (var pieceCornerIndex in pieceCornerIndicies)
                    {
                        var pieceCornerPos = piece.Variants[variant].Squares[pieceCornerIndex];

                        foreach (var boardCorner in corners)
                        {
                            var piecePos = boardCorner - pieceCornerPos;
                            bool isOk = true;

                            foreach (var square in piece.Variants[variant].Squares)
                            {
                                var cell = GetBoardExtCell(boardExt, piecePos, square);
                                if (cell != BoardCell.Empty && cell != BoardCell.Corner)
                                {
                                    isOk = false;
                                    break;
                                }
                            }
                            if (isOk)
                            {
                                result.Add(new Move()
                                {
                                    Piece = piece,
                                    VariantNumber = variant,
                                    Position = piecePos
                                });
                                moveCount--;
                                if (moveCount <= 0)
                                {
                                    return result;
                                }
                            }

                        }
                    }
                }
            }
            return result;
        }

        private static BoardCell GetBoardExtCell(BoardCell[,] boardExt, PiecePosition piecePos, PiecePosition square)
        {
            int x = square.X + piecePos.X;
            int y = square.Y + piecePos.Y;
            if (x < 0 || y < 0 || x >= Board.BoardSize || y >= Board.BoardSize)
            {
                return BoardCell.OutOfBoard;
            }
            return boardExt[x, y];
        }

        private static List<PiecePosition> CreateBoardCornersList(BoardCell[,] boardExt)
        {
            var corners = new List<PiecePosition>();
            for (int i = 0; i < Board.BoardSize; i++)
            {
                for (int j = 0; j < Board.BoardSize; j++)
                {
                    if (boardExt[i, j] == BoardCell.Corner)
                    {
                        corners.Add(new PiecePosition(i, j));
                    }
                }
            }
            return corners;
        }

        private static void FillCorners(GameState gameState, BoardCell[,] boardExt)
        {
            for (int i = 0; i < Board.BoardSize; i++)
            {
                for (int j = 0; j < Board.BoardSize; j++)
                {
                    if (boardExt[i, j] != BoardCell.Empty)
                    {
                        continue;
                    }
                    bool corner = ExistCorner(i, j, boardExt, (BoardCell)gameState.CurrentPlayerColor);
                    bool side = ExistSide(i, j, boardExt, (BoardCell)gameState.CurrentPlayerColor);

                    if (side)
                    {
                        boardExt[i, j] = BoardCell.Prohibited;
                    }
                    else if (corner)
                    {
                        boardExt[i, j] = BoardCell.Corner;
                    }
                }
            }
        }

        private static BoardCell[,] CreateBoard(GameState gameState)
        {
            var boardExt = new BoardCell[Board.BoardSize, Board.BoardSize];
            for (int i = 0; i < Board.BoardSize; i++)
            {
                for (int j = 0; j < Board.BoardSize; j++)
                {
                    boardExt[i, j] = (BoardCell)gameState.Board.BoardElements[i, j];
                }
            }
            return boardExt;
        }

        private static bool ExistCorner(int x, int y, BoardCell[,] board, BoardCell kind)
        {
            return
                IsCellEqual(x - 1, y - 1, board, kind) ||
                IsCellEqual(x + 1, y - 1, board, kind) ||
                IsCellEqual(x - 1, y + 1, board, kind) ||
                IsCellEqual(x + 1, y + 1, board, kind);
        }

        private static bool ExistSide(int x, int y, BoardCell[,] board, BoardCell kind)
        {
            return
                IsCellEqual(x - 1, y, board, kind) ||
                IsCellEqual(x + 1, y, board, kind) ||
                IsCellEqual(x, y + 1, board, kind) ||
                IsCellEqual(x, y - 1, board, kind);
        }

        private static bool IsCellEqual(int x, int y, BoardCell[,] board, BoardCell kind)
        {
            return x >= 0 && y >= 0 && x < Board.BoardSize && 
                y < Board.BoardSize && board[x, y] == kind;
        }

        /// <summary>
        /// Wywoływane gdy aktualny gracz nie ma żadnych możliwych ruchów.
        /// </summary>
        /// <param name="gameState">Bieżący Stan gry</param>
        /// <returns>-1 gdy aktualny gracz przegrał, +1 gdy wygrał, 0 gdy remis</returns>
        public static double GetGameResult(GameState gameState)
        {
            Player blocked = gameState.CurrentPlayerColor;
            Player opponent = blocked == Player.Orange ? Player.Violet : Player.Orange;

            int blockedScore = GetPlayerScore(gameState, blocked);
            int opponentScore = GetPlayerScore(gameState, opponent);
            int result = -1; //domyślnie gracz zablokowany przegrywa

            if (blockedScore >= opponentScore)
            //stawiaj klocki na planszy niezablokowanym graczem dopóki nie ma większego wyniku. 
            //zablokowany gracz wygrywa tylko gdy dla kazdego polozenia klockow gracza niezablokowanego gracz zablokowany ma wiekszy wynik
            {
                gameState.SwapCurrentPlayer();
                result = PlacePieces(gameState, opponent, blockedScore);
                gameState.SwapCurrentPlayer();
            }            

            return result;

        }

        /// <summary>
        /// Układa na planszy klocki niezablokowanego gracza dopóki ma on mniejszy wynik od zablokowanego gracza
        /// </summary>
        /// <returns>-1 jeśli niezablokowany gracz wygrał, 1 jeśli przegrał, 0 jeśli zremisował 
        /// czyli wynik gry z punktu widzenia zablokowanego gracza</returns>
        private static int PlacePieces(GameState gameState, Player opponent, int blockedScore)
        {
            var moves = GetMoves(gameState);
            int opponentScore = GetPlayerScore(gameState, opponent);
            int result = opponentScore==blockedScore? 0:1;
            foreach (var move in moves)
            {
                gameState.AddMove(move);
                opponentScore = GetPlayerScore(gameState, opponent);
                if (opponentScore > blockedScore)
                {
                    result = -1;
                }
                else
                {
                    if (opponentScore == blockedScore)
                    {
                        result = 0;
                    }
                    else
                    {
                        result = Math.Min(result, PlacePieces(gameState, opponent, blockedScore));
                    }
                }
                gameState.DelMove(move);
                if (result == -1)
                {
                    break;
                }
            }
            return result;
        }

        private static int GetPlayerScore(GameState gameState, Player player)
        {
            if (player == Player.Violet)
            {
                return GetVioletScore(gameState);
            }
            if (player == Player.Orange)
            {
                return GetOrangeScore(gameState);
            }
            return 0;
        }

        private static int GetOrangeScore(GameState gameState)
        {
            int orangeScore = 0;

            orangeScore -= (from o in gameState.OrangeHand.HandPieces select o.Variants[0].Squares.Length).Sum();

            if (orangeScore == 0) //wszyskie klocki położył
            {
                orangeScore += EmptyHandBonus;
                if (gameState.IsLastOrangeMoveMonomino)
                {
                    orangeScore += MonominoLastBonus;
                }
            }
            return orangeScore;
        }

        private static int GetVioletScore(GameState gameState)
        {
            int violetScore = 0;

            violetScore -= (from o in gameState.VioletHand.HandPieces select o.Variants[0].Squares.Length).Sum();
            if (violetScore == 0) //wszyskie klocki położył
            {
                violetScore += EmptyHandBonus;
                if (gameState.IsLastVioletMoveMonomino)
                {
                    violetScore += MonominoLastBonus;
                }
            }
            return violetScore;
        }

        public static Player GetWinner(GameState gameState)
        {
            int orangeScore = GetOrangeScore(gameState);
            int violetScore = GetVioletScore(gameState);            

            if (orangeScore == violetScore)
            {
                return Player.None;
            }
            if (orangeScore > violetScore)
            {
                return Player.Orange;
            }
            return Player.Violet;
        }
    }

    enum BoardCell : int
    {
        Empty = 0,
        Orange = 1,
        Violet = 2,
        Prohibited = 3,
        Corner = 4,
        OutOfBoard = 5
    }
}
