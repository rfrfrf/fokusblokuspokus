using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.MonteCarloTreeSearch
{
    class MonteCarloTreeSearchPlayer : AIPlayer
    {
        public override void OnGameStart()
        {
            //TODO: inicjalizacja, wczytywanie wyuczonego drzewka?
        }

        public override void OnGameEnd()
        {
            //TODO: zapis wyuczonego drzewka?
        }

        public override Move GetMove(GameState gameState)
        {
            return null; //TODO: wyszukiwanie ruchu przy pomocy MCTS
        }

        public override string ToString()
        {
            return "Monte Carlo Tree Search";
        }
    }
}
