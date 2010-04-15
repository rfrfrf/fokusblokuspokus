using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.AlphaBeta
{
    class AlphaBetaHeuristics
    {
        /// <summary>
        /// dla pomaranczowego gracza promuje ruchy w kierunku lewego gornego rogu planszy
        /// dla fioletowego gracza promuje ruchy w kierunku prawego dolnego rogu planszy
        /// </summary>
        public double GetBoardEvaluation(GameState gameState)
        {
            var elements = gameState.Board.BoardElements;
            double result = 0.0;

            if (gameState.CurrentPlayerColor == Player.Orange)
            {
                for (int i = 0; i < elements.GetLength(0); i++)
                {
                    for (int j = 0; j < elements.GetLength(1); j++)
                    {
                        if (elements[i, j] == Player.Orange)
                        {
                            result += 1.0 / (i + j+ 5);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < elements.GetLength(0); i++)
                {
                    for (int j = 0; j < elements.GetLength(1); j++)
                    {
                        if (elements[i, j] == Player.Violet)
                        {
                            result += 1.0 / (elements.GetLength(0) - i + elements.GetLength(1)-j+5);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// na poczatku listy klockow ustawia klocki o najwiekszej liczbie rogow lub najwiekszej liczbie elementow
        /// </summary>
        /// <param name="gameState"></param>
        public void SortHand(GameState gameState)
        {           
            gameState.CurrentPlayerHand.HandPieces.Sort((x, y) =>
            {
                int a = x.Variants[0].CornersIndicies.Length;
                int b = y.Variants[0].CornersIndicies.Length;
                if (a != b)
                {
                    return b.CompareTo(a);
                }
                return y.Variants[0].Squares.Length.CompareTo(x.Variants[0].Squares.Length);
            });
        }

        /// <summary>
        /// dla pomaranczowego gracza promuje ruchy w kierunku lewego gornego rogu planszy
        /// dla fioletowego gracza promuje ruchy w kierunku prawego dolnego rogu planszy
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="moves"></param>
        public void SortMoves(GameState gameState, List<Move> moves)
        {
            int multiplier = gameState.CurrentPlayerColor == Player.Violet ? 1 : -1;
            moves.Sort((x, y) =>
            {
                int a = x.Position.X + x.Position.Y;
                int b = y.Position.X + y.Position.Y;
                if (a != b)
                {
                    return b.CompareTo(a) * multiplier;
                }
                return y.PieceVariant.Squares.Length.CompareTo(x.PieceVariant.Squares.Length) * multiplier;
            });
        }
    }
}
