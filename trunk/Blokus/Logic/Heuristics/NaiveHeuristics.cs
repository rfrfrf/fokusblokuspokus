using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.Heuristics
{
    class NaiveHeuristics : HeuristicsBase
    {       
        public override double GetBoardEvaluation(GameState gameState)
        {
            double add = 0.0;
            if (IsNonDeterministic)
            {
                add = _Random.NextDouble();
                add *= add;
                add *= add;
            }

            int result = GameRules.CountCurrentPlayerCorners(gameState); // dodaj dostepne narozniki gracza
            gameState.SwapCurrentPlayer();
            result -= GameRules.CountCurrentPlayerCorners(gameState); // i odejmij narozniki przeciwnika
            gameState.SwapCurrentPlayer();

            return result * (1.0 + add) * 0.001;
        }

        /// <summary>
        /// na poczatku listy klockow ustawia klocki o najwiekszej liczbie rogow lub najwiekszej liczbie elementow
        /// </summary>
        /// <param name="gameState"></param>
        public override void SortHand(GameState gameState)
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
            if (IsNonDeterministic)
            {
                Randomize(gameState.CurrentPlayerHand.HandPieces);
            }
        }



        public static void Randomize<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count / 5; i++)
            {
                int a = _Random.Next(list.Count);
                int b = _Random.Next(list.Count);

                var tmp = list[a];
                list[a] = list[b];
                list[b] = tmp;
            }
        }

        /// <summary>
        /// dla pomaranczowego gracza promuje ruchy w kierunku lewego gornego rogu planszy
        /// dla fioletowego gracza promuje ruchy w kierunku prawego dolnego rogu planszy
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="moves"></param>
        public override void SortMoves(GameState gameState, List<Move> moves)
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
            if (IsNonDeterministic)
            {
                Randomize(moves);
            }
        }

        public override string ToString()
        {
            return "Naive";
        }
    }
}
