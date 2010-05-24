using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.Heuristics;

namespace Blokus.Logic.RandomPlayer
{
    class RandomPlayer:AIPlayer
    {
        private Random _Random = new Random();

        public override HeuristicsBase Heuristics { get; set; }

        public override Move GetMove(GameState gameState)
        {
            var moves = GameRules.GetMoves(gameState);
            if (moves.Count == 0)
            {
                return null;
            }
            return moves[_Random.Next(moves.Count)];
        }

        public override string ToString()
        {
            return "Random";
        }
    }
}
