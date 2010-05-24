using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.Heuristics;

namespace Blokus.Logic
{
    [Serializable]
    public abstract class PlayerBase
    {
        public abstract Move GetMove(GameState gameState);
        public abstract void OnGameStart(GameState gameState);
        public abstract void OnGameEnd(GameState gameState);
        public abstract void CancelMove(GameState gameState);
        public abstract HeuristicsBase Heuristics { get; set; }
    }
}
