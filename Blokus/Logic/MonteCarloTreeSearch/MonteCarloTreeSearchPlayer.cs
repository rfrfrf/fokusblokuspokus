using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.AlphaBeta;

namespace Blokus.Logic.MonteCarloTreeSearch
{
    class MonteCarloTreeSearchPlayer : AIPlayer
    {
        public const string filename = "tree.dat";
        //Player me;
        public MultipleTree tree;//=new MultipleTree();
        public static AlphaBetaPlayer player = new AlphaBetaPlayer();

        public int MaxDepth
        {
            get { return player.MaxDepth; }
            set { player.MaxDepth = value; }
        }

        public int MaxTreeRank
        {
            get { return player.MaxTreeRank; }
            set { player.MaxTreeRank = value; }
        }

        public override void OnGameStart()
        {
            //TODO: inicjalizacja, wczytywanie wyuczonego drzewka?
            tree = new MultipleTree();
            //tree.mePlayer = me;
            //tree.ReadTree(filename);

        }

        public override void OnGameEnd()
        {
            //TODO: zapis wyuczonego drzewka?
            //tree.SaveTree(filename);
        }

        public override Move GetMove(GameState gameState)
        {
            //tree.mePlayer = me = gameState.CurrentPlayerColor;
            //tree.SelectNodeFromSubTree(tree.root, 
            return tree.MakeMove(gameState);
            //return null; //TODO: wyszukiwanie ruchu przy pomocy MCTS
        }

        public override string ToString()
        {
            return "Monte Carlo Tree Search";
        }
    }
}
