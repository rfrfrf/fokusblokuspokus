using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace Blokus.Logic.MCTS2v2
{
    [Serializable]
    public class MCTS2v2Player:AIPlayer
    {

        private static Node _Root;
        //private Player me;

        public override void OnGameStart(GameState gameState)
        {
            //me = gameState.OrangePlayer is MCTS2v2Player ? (gameState.VioletPlayer is MCTS2v2Player ? Player.None : Player.Orange) : Player.Violet;
            base.OnGameStart(gameState);
        }



        public override Move GetMove(GameState gameState)
        {
            MCTSSolver(gameState, _Root);
            return SelectOptimumMoveToPut(gameState);
        }

        

        private int MCTSSolver(GameState gameState, Node node)
        {
            List<Move> moves = GameRules.GetMoves(gameState);
            if (moves.Count == 0)//czyli koniec gry, sprawdzenie kto wygrał
            {
                Player p = GameRules.GetWinner(gameState);
                if (p == Player.None)//remis
                {
                    return 0;
                }
                else if (p == gameState.CurrentPlayerColor)//ja wygralem
                {
                    return int.MaxValue;
                }
                else //if(p== Player.Violet)//czyli ja przegralem
                {
                    return int.MinValue;
                }
            }
            //gra trwa

            Node bestchild;
            Move bestchildmove;
            Select(node, out bestchild, out bestchildmove);
            node.VisitCount++;

            int R = 0;

            if (bestchild.value != int.MinValue && bestchild.value != int.MaxValue)
            {
                if (bestchild.VisitCount == 0)
                {
                    R = -Playout(bestchild);
                    node.AddChild(bestchildmove.SerializedMove, bestchild);
                    node.computeAverage(R);
                    return R;
                }
                else
                {
                    gameState.AddMove(bestchildmove);
                    gameState.SwapCurrentPlayer();
                    R = -MCTSSolver(gameState, bestchild);//chyba należałoby wcześniej zmienić gameState??? czy w taki sposob
                    gameState.SwapCurrentPlayer();
                    gameState.DelMove(bestchildmove);
                }
            }
            else
            {
                R = bestchild.value;
            }

            if (R == int.MaxValue)
            {
                node.value = int.MinValue;
                return R;
            }
            else if (R == int.MinValue)
            {
                foreach (var c in node.Children)
                {
                    if (c.Value.value != R)
                    {
                        R = -1;
                        node.computeAverage(R);
                        return R;
                    }
                }
                node.value = int.MaxValue;
                return R;
            }

            node.computeAverage(R);
            return R;


        }

        private void Select(Node node, out Node bestchild, out Move bestchildmove)
        {
            throw new NotImplementedException();
        }

        private int Playout(Node bestchild)
        {
            throw new NotImplementedException();
        }

        private Move SelectOptimumMoveToPut(GameState gameState)
        {
            throw new NotImplementedException();
        }


        








        public static void SaveTree(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, _Root);
            fs.Close();
        }

        public static void ReadTree(string filename)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                _Root = (Node)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
                _Root = new Node() { VisitCount = 1 };
                MessageBox.Show("Nie udało się wczytać drzewa. Utworzono nowe.");
            }

        }





    }
}
