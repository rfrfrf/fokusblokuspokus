using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.MCTS2v2
{
    public static class GameStateExtender
    {
        public static GameState Clone(this GameState g)//brak obsługi zablokowanego gracza
        {
            GameState gs = new GameState();
            bool blocked = false;
            foreach (Move m in g.AllMoves)
            {

                try
                {
                    gs.AddMove(m);
                }
                catch (Exception)
                {
                    blocked = true;
                    gs.SwapCurrentPlayer();
                }
                if (!blocked)
                {
                    gs.SwapCurrentPlayer();
                }
            }
            return gs;
        }
    }
}
