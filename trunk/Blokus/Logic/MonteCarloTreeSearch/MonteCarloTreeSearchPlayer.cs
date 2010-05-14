﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.MonteCarloTreeSearch
{
    class MonteCarloTreeSearchPlayer : AIPlayer
    {
        string filename = "tree.dat";
        Player me;
        MultipleTree tree;
        public override void OnGameStart()
        {
            //TODO: inicjalizacja, wczytywanie wyuczonego drzewka?
            tree = new MultipleTree();
            tree.mePlayer = me;
            tree.ReadTree(filename);

        }

        public override void OnGameEnd()
        {
            //TODO: zapis wyuczonego drzewka?
            tree.SaveTree(filename);
        }

        public override Move GetMove(GameState gameState)
        {
            tree.mePlayer = me = gameState.CurrentPlayerColor;
            //tree.SelectNodeFromSubTree(tree.root, 
            return null; //TODO: wyszukiwanie ruchu przy pomocy MCTS
        }

        public override string ToString()
        {
            return "Monte Carlo Tree Search";
        }
    }
}
