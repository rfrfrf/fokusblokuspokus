using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.AlphaBeta
{
    class AlphaBetaHeuristics : Heuristics
    {
        private bool nonDeterministic = false;
        private static Random _Random = new Random();

        public AlphaBetaHeuristics()
        {
        }

        public AlphaBetaHeuristics(bool nonDeterministic)
        {
            this.nonDeterministic = nonDeterministic;
        }

        public override double GetBoardEvaluation(GameState gameState)
        {
            double add =0.0;
            if (nonDeterministic)
            {
                add = _Random.NextDouble();
                add *= add;
                add *= add;
            }

            return GameRules.GetMoves(gameState).Count * 0.0001 * (1 + add);
        }

  /*      /// <summary>
        /// dla pomaranczowego gracza promuje ruchy w kierunku lewego gornego rogu planszy
        /// dla fioletowego gracza promuje ruchy w kierunku prawego dolnego rogu planszy
        /// </summary>
        public override double GetBoardEvaluation(GameState gameState)
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
        }*/

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
            if (nonDeterministic)
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
            if (nonDeterministic)
            {
                Randomize(moves);
            }
        }
        /*
        /// <summary>
        /// Ustawia na początku te ruchy, które skutkują najmniejszą ilością ruchów przeciwnika
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="moves"></param>
        public override void SortMoves(GameState gameState, List<Move> moves)
        {
            Dictionary<Move, int> oponentMovesCount = new Dictionary<Move, int>();
            for (int i = 0; i < moves.Count; i++)
            {
                gameState.AddMove(moves[i]);
                gameState.SwapCurrentPlayer();
                oponentMovesCount[moves[i]] = GameRules.GetMoves(gameState).Count;
                gameState.SwapCurrentPlayer();
                gameState.DelMove(moves[i]);
            }
            moves.Sort((x, y) =>
            {
                return oponentMovesCount[x].CompareTo(oponentMovesCount[y]);
            });
        }*/
    }
}
