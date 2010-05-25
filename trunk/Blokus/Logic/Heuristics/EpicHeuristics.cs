using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.Heuristics
{
    class EpicHeuristics : HeuristicsBase
    {
        private int[] indices;
        private static HashSet<int> Phase1Set = new HashSet<int> { 12, 15, 16, 19 };
        private static HashSet<int> Phase2Set = new HashSet<int> { 21, 18 };
        private static HashSet<int> Phase3Set = new HashSet<int> { 20, 11, 17, 14, 10, 13 };

        public EpicHeuristics()
        {
            CreateOrder();
        }

        private void CreateOrder()
        {
            int[] order = new int[] { 19, 15, 16, 12, 18, 21, 11, 17, 20, 13, 14, 10, 8, 5, 6, 3, 9, 7, 4, 2, 1 };
            indices = new int[order.Length];
            for (int i = 0; i < order.Length; i++)
            {
                indices[order[i] - 1] = i;
            }
        }

        public override double GetBoardEvaluation(GameState gameState)
        {
            double add = 0.0;
            if (IsNonDeterministic)
            {
                add = _Random.NextDouble();
                add *= add;
                add *= add;
            }

            return GameRules.GetMoves(gameState).Count * 0.001 * (1 + add);
        }


        public override void SortHand(GameState gameState)
        {
            gameState.CurrentPlayerHand.HandPieces.Sort((x, y) =>
            {
                return indices[x.Id-1].CompareTo(indices[y.Id-1]);
            });
        }


        public override void SortMoves(GameState gameState, List<Move> moves)
        {
            int phase = 1;
            if (gameState.AllMoves.Count >= 8)
            {
                phase = 2;
            }
            if (gameState.AllMoves.Count >= 12)
            {
                phase = 3;
            }
            switch(phase)
            {
                case 1: Phase1(gameState, moves); break;
                case 2: Phase2(gameState, moves); break;
                case 3: Phase3(gameState, moves); break;
            }
        }

        private static void Phase1(GameState gameState, List<Move> moves)
        {
            int halfsize = Board.BoardSize / 2 - 1;
            moves.Sort((x, y) =>
            {
                int a = Math.Abs(halfsize - x.Position.X) + Math.Abs(halfsize - x.Position.Y);
                int b = Math.Abs(halfsize - y.Position.X) + Math.Abs(halfsize - y.Position.Y);

                if (!Phase1Set.Contains(x.Piece.Id))
                {
                    a += 1000;
                }
                if (!Phase1Set.Contains(y.Piece.Id))
                {
                    b += 1000;
                }
                if (a != b)
                {
                    return a.CompareTo(b);
                }
                return y.PieceVariant.Squares.Length.CompareTo(x.PieceVariant.Squares.Length);
            });
        }

        private static void Phase2(GameState gameState, List<Move> moves)
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

        private static void Phase3(GameState gameState, List<Move> moves)
        {
            moves.Sort((x, y) =>
            {
                int a = Math.Abs(Board.BoardSize - 2 - x.Position.X - x.Position.Y);
                int b = Math.Abs(Board.BoardSize - 2 - y.Position.X - y.Position.Y);

                if (!Phase3Set.Contains(x.Piece.Id))
                {
                    a += 1000;
                }
                if (!Phase3Set.Contains(y.Piece.Id))
                {
                    b += 1000;
                }

                if (a != b)
                {
                    return a.CompareTo(b);
                }
                return y.PieceVariant.Squares.Length.CompareTo(x.PieceVariant.Squares.Length);
            });
        }

        public override string ToString()
        {
            return "Epic";
        }
    }
}
