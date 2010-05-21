using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.MCTS2v2
{
    public static class GameStateExtender
    {
        public static GameState Clone(this GameState g)
        {
            GameState gs = new GameState();
            foreach (Move m in g.AllMoves)
            {
                gs.AddMove(m);
                gs.SwapCurrentPlayer();
            }
            return gs;
        }
    }
}
