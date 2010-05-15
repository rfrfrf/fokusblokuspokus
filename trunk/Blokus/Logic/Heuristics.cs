using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    [Serializable]
    public abstract class Heuristics
    {
        public abstract double GetBoardEvaluation(GameState gameState);
        public abstract void SortHand(GameState gameState);
        public abstract void SortMoves(GameState gameState, List<Move> moves);
    }
}
