using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.Heuristics
{
    [Serializable]
    public abstract class HeuristicsBase
    {
        public abstract double GetBoardEvaluation(GameState gameState);
        public abstract void SortHand(GameState gameState);
        public abstract void SortMoves(GameState gameState, List<Move> moves);
        public bool IsNonDeterministic { get; set; }

        protected static Random _Random = new Random();
    }
}
