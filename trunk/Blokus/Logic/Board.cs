using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Blokus.Logic
{
    public class Board
    {
        public const int BoardSize = 14;
        public Player[,] BoardElements { get; set; }

        public Board()
        {
            BoardElements = new Player[BoardSize, BoardSize];
        }

        public Board(int sizex, int sizey)
        {
            BoardElements = new Player[sizex, sizey];
        }

        public Player GetElementAt(PiecePosition point)
        {
            return BoardElements[point.X, point.Y];
        }

        public void SetElementAt(PiecePosition point, Player value)
        {
            BoardElements[point.X, point.Y] = value;
        }

        public void PlacePiece(Move move, Player moveExecutor)
        {
            foreach (var square in move.PieceVariant.Squares)
            {
                SetElementAt(square + move.Position, moveExecutor);
            }
        }

     /*   public void PlacePieceWithHighlightedCorners(Move move, Player moveExecutor)
        {
            for (int i = 0; i < move.PieceVariant.Squares.Length; i++ )
            {
                var square = move.PieceVariant.Squares[i];
                bool highlighted = move.PieceVariant.CornersIndicies.Contains(i);
                int player = highlighted ? (int)moveExecutor : (int)moveExecutor + 2;
                SetElementAt(square + move.Position, (Player)player);
            }
        }*/

        public void RemovePiece(Move move)
        {
            foreach (var square in move.PieceVariant.Squares)
            {
                SetElementAt(square + move.Position, Player.None);
            }
        }

        public Board Clone()
        {
            var result = new Board(BoardElements.GetLength(0), BoardElements.GetLength(1));
            for (int i = 0; i < BoardElements.GetLength(0); i++)
            {
                for (int j = 0; j < BoardElements.GetLength(1); j++)
                {
                    result.BoardElements[i, j] = BoardElements[i, j];
                }
            }
            return result;
        }
    }
}
