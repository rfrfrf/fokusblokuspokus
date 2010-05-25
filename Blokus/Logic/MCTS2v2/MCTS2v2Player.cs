using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using Blokus.Logic.Scout;
using Blokus.Logic.Heuristics;

namespace Blokus.Logic.MCTS2v2
{
    [Serializable]
    public class MCTS2v2Player : AIPlayer
    {
        HeuristicsBase heuristics;
        private static Node _Root;
        //private Player me;
        private Random _Random = new Random();
        private delegate double MaximumFormula(Node n, int move, GameState gameState);
        private Node currentNode, currentNodeParent;
        private ScoutPlayer simulationStrategy = new ScoutPlayer();
        private const int visitTreshhold = 5;
        private const double Cvalue = 1;

        private const double Avalue = 1;

        private int lastMovesCount = -1;

        List<Node> prevNodes;
        //List<int> movesCountPerNode;//zawiera info o tym, ile bylo dozwolonych ruchow dla node w prevNodes
        private int movesWhenOpponentBlocked;


        public override HeuristicsBase Heuristics
        {
            get { return heuristics; }
            set 
            { 
                heuristics = value;
                simulationStrategy.Heuristics = value;
            }
        }

        public int MaxDepth
        {
            get { return simulationStrategy.MaxDepth; }
            set { simulationStrategy.MaxDepth = value; }
        }

        public int MaxTreeRank
        {
            get { return simulationStrategy.MaxTreeRank; }
            set { simulationStrategy.MaxTreeRank = value; }
        }



        public override void OnGameStart(GameState gameState)
        {
            //me = gameState.OrangePlayer is MCTS2v2Player ? (gameState.VioletPlayer is MCTS2v2Player ? Player.None : Player.Orange) : Player.Violet;
            currentNode = _Root;
            currentNodeParent = null;
            //simulationStrategy.MaxDepth = 1;
            //simulationStrategy.MaxTreeRank = 1;
            prevNodes = new List<Node>();
            //movesCountPerNode = new List<int>();
            prevNodes.Add(currentNode);
            movesWhenOpponentBlocked = int.MaxValue;
            lastMovesCount = -1;
            //base.OnGameStart(gameState);
        }



        public override Move GetMove(GameState gameState)
        {
            List<Move> mmmoves = GameRules.GetMoves(gameState);
            if (currentNode == null && gameState.AllMoves.Count > 2 || mmmoves.Count == 0)
            {
                return null;
            }


            if (gameState.AllMoves.Count != 0 && currentNode != null)
            {
                int serMove = gameState.AllMoves[gameState.AllMoves.Count - 1].SerializedMove;
                if (currentNode.Children != null && currentNode.Children.ContainsKey(serMove))
                {
                    currentNodeParent = currentNode;
                    currentNode = currentNode.Children[serMove];
                    prevNodes.Add(currentNode);
                }
                else
                {
                    //albo nie ma ruchu bo nie bylo go w drzewie, albo przeciwnik jest zablokowany
                    if (lastMovesCount != gameState.AllMoves.Count)
                    {
                        currentNodeParent = currentNode;
                        Node pom = new Node();//currentNode);
                        currentNode.AddChild(serMove, pom);
                        currentNode = pom;
                        prevNodes.Add(currentNode);
                    }
                    else
                    {
                        if (movesWhenOpponentBlocked == int.MaxValue)
                        {
                            movesWhenOpponentBlocked = gameState.AllMoves.Count;
                        }
                    }
                }
            }
            //currentNode ostatni na liscie prevNodes
            lastMovesCount = gameState.AllMoves.Count;
            int R = MCTSSolver(gameState, currentNode, currentNodeParent);//teraz currentNode nie jest zaktualizowany
            BackpropagateToRoot(R);
            Node pomn = null;
            Move res = SelectOptimumMoveToPut(gameState, currentNode, out pomn);
            if (res != null)
            {
                currentNode = pomn;
                prevNodes.Add(currentNode);
            }
            else
            {
                currentNode = null;
            }
            return res;

        }

        private void BackpropagateToRoot(int R)
        {
            if (prevNodes.Count <= 0)
            {
                return;
            }
            Node node = null;
            for (int i = prevNodes.Count - 1; i > 0; i--)//od --przed--ostatniego gdyż ostatni został zaktualizowany przed wywołaniem tej funkcji
            {
                node = prevNodes.ElementAt(i);

                bool rev = false;
                R = ReverseOrNotR(R, i, out rev);//nie odwracamy wtw gdy przeciwnik jest zablokowany
                //teraz aktualizacja!!!

                //nalezy sprawdzic czy nie propaguje sie int.MaxValue lub int.MinValue wtw gdy MCTSSolver moze zwrocic int.MaxValue lub int.MinValue
                //if (R == int.MaxValue || R == int.MinValue)
                //{

                //}
                node.computeAverage(R);

                //if (R == int.MaxValue)
                //{
                //    node.value = rev ? int.MinValue : int.MaxValue;
                //    //R *= -1;
                //    continue;
                //}
                //else if (R == int.MinValue)
                //{
                //    CheckChildren(ref R, node, rev);//ta funkcja moze zmienic R z minValue na 1 lub -1
                //    //R *= -1;
                //    continue;
                //}

                //node.computeAverage(R);
                ////R *= -1;
                ////continue;




                
                
            }
        }

        private int ReverseOrNotR(int R, int i, out bool rev)
        {
            return R = (rev = i < movesWhenOpponentBlocked-1) ? reversedR(R) : R;
        }

        private int reversedR(int R)
        {
            if (R == int.MinValue)
            {
                return int.MaxValue;
            }
            else if (R == int.MaxValue)
            {
                return int.MinValue;
            }
            else
            {
                return -R;
            }
        }

        private static void CheckChildren(ref int R, Node node, bool rev)
        {
            foreach (var c in node.Children)
            {
                if (c.Value.value != int.MinValue)
                {
                    R = rev ? 1 : -1;// -1;
                    node.computeAverage(R);
                    return;// R;
                }
            }
            node.value = rev ? int.MaxValue : int.MinValue;
            //return R;
        }



        private int MCTSSolver(GameState gameState, Node node, Node parent)
        {
            heuristics.SortHand(gameState);
            List<Move> moves = GameRules.GetMoves(gameState);
            heuristics.SortMoves(gameState, moves);
            if (moves.Count == 0)//czyli koniec gry, sprawdzenie kto wygrał
            {
                var p = GameRules.GetGameResult(gameState); 
                if (p == 0)//remis
                {
                    return 0;
                }
                else if (p == 1)//ja wygralem
                {
                    return 1;// int.MaxValue;
                }
                else //if(p== Player.Violet)//czyli ja przegralem
                {
                    return -1;// int.MinValue;
                }
            }
            //gra trwa

            Node bestchild;
            Move bestchildmove;
            Select(gameState, node, parent, out bestchild, out bestchildmove, moves);
            node.VisitCount++;

            int R = 0;

            if (bestchild.value != int.MinValue && bestchild.value != int.MaxValue)
            {
                if (bestchild.VisitCount == 0)
                {
                    gameState.AddMove(bestchildmove);
                    gameState.SwapCurrentPlayer();
                    R = -PlaySimulatedGame(gameState); //być może bez minusa
                    //zakladamy, ze R z perspektywy node'a
                    gameState.SwapCurrentPlayer();
                    gameState.DelMove(bestchildmove);
                    node.AddChild(bestchildmove.SerializedMove, bestchild);
                    //node.computeAverage(R);
                    bestchild.computeAverage(R);
                    bestchild.VisitCount++;
                    return R;
                }
                else
                {
                    gameState.AddMove(bestchildmove);
                    gameState.SwapCurrentPlayer();
                    R = reversedR(MCTSSolver(gameState, bestchild, node));//chyba należałoby wcześniej zmienić gameState??? czy w taki sposob
                    //R z perspktywy node'a
                    gameState.SwapCurrentPlayer();
                    gameState.DelMove(bestchildmove);
                }
            }
            else
            {
                R = bestchild.value;
                //R z perspektywy node'a
            }

            //R z perspektywy node'a


            if (R == int.MaxValue)
            {
                if (moves.Count != node.Children.Count)
                {
                    return 1;
                }
                //node.value = int.MinValue;
                return R;
            }
            else if (R == int.MinValue)
            {
                if (moves.Count != node.Children.Count)
                {
                    return -1;
                }
                    foreach (var c in node.Children)
                    {
                        if (c.Value.value != R)
                        {
                            R = -1;
                            //node.computeAverage(R);
                            return R;
                        }
                    }
                
                //node.value = int.MaxValue;
                return R;
            }

            //node.computeAverage(R);
            return R;


        }

        private void Select(GameState gs, Node node, Node parent, out Node bestchild, out Move bestchildmove, List<Move> moves)
        {
            //Node pomNode = null;
            int pomMove = 0;
            FindMaximizedNode(gs, node, (n, move, gameState) =>
            {

                if (n.VisitCount >= visitTreshhold)
                {
                    return (n.value + Math.Sqrt(Cvalue * Math.Log(parent != null ? parent.VisitCount : 1, Math.E) / n.VisitCount));
                }
                else
                {
                    gameState.AddMove(new Move(move));
                    gameState.SwapCurrentPlayer();

                    var result = -heuristics.GetBoardEvaluation(gameState);

                    gameState.SwapCurrentPlayer();
                    gameState.DelMove(new Move(move));
                    return result;
                }


                ;
            }


                , out bestchild, out pomMove, moves);
            bestchildmove = new Move(pomMove);
        }

        //private int Playout(Node bestchild)
        //{
        //    throw new NotImplementedException();
        //}

        private Move SelectOptimumMoveToPut(GameState gameState, Node currNode, out Node nextNode)
        {
            //Node pomNode=null;
            int pommove = 0;
            FindMaximizedNode(null, currentNode, (node, move, gs) => { return (node.value + Avalue / Math.Sqrt(node.VisitCount)); }, out nextNode, out pommove, null);
            return pommove != 0 ? new Move(pommove) : null;
        }

        private int PlaySimulatedGame(GameState gstate)
        {
            GameState state = gstate.Clone();

            while (true)
            {
                //var moves = GameRules.GetMoves(state);
                //if (moves.Count == 0)
                //{
                //    break;
                //}
                var move = simulationStrategy.GetMove(state);
                if (move == null)
                {
                    break;
                }
                state.AddMove(move);
                state.SwapCurrentPlayer();
            }
            var result = (int)GameRules.GetGameResult(state);
            if (state.CurrentPlayerColor != gstate.CurrentPlayerColor)
            {
                result *= -1;
            }
            return result;
        }


        /// <summary>
        /// Znajduje najlepsze dziecko wg formuly mf
        /// Gdy gs==null to szuka tylko wśród dzieci w drzewie
        /// </summary>
        /// <param name="gs"></param>
        /// <param name="toLook"></param>
        /// <param name="mf"></param>
        /// <param name="n"></param>
        /// <param name="move"></param>
        private void FindMaximizedNode(GameState gs, Node toLook, MaximumFormula mf, out Node n, out int move, List<Move> moves)
        {
            double maxVal = double.NegativeInfinity, currVal = double.NegativeInfinity;
            n = null;
            move = 0;
            if (toLook.Children != null)
            {
                foreach (var d in toLook.Children)
                {
                    currVal = mf(d.Value, d.Key, gs);
                    if (currVal > maxVal)
                    {
                        maxVal = currVal;
                        n = d.Value;
                        move = d.Key;
                    }
                }
            }
            if (gs != null )//&& maxVal<0) //jak nie jest ujemne to i tak nic nie znajdzie
            {
                if (moves == null)
                {
                    heuristics.SortHand(gs);
                    moves = GameRules.GetMoves(gs);
                    heuristics.SortMoves(gs, moves);
                }
                if (toLook.Children != null && toLook.Children.Count == moves.Count)
                {
                    return;
                }
                foreach (Move m in moves)
                {
                    if (toLook.Children == null || !toLook.Children.ContainsKey(m.SerializedMove))//jeśli nie zawiera
                    {
                        Node nd = new Node();//toLook);
                        currVal = mf(nd, m.SerializedMove, gs);// i tak zwraca zero
                        if (currVal > maxVal)
                        {
                            maxVal = currVal;
                            n = nd;
                            move = m.SerializedMove;
                          //  break; //i tak nie znajdzie niczego większego
                        }
                    }
                }
            }            
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
                MessageBox.Show("MCTS2v2: Nie udało się wczytać drzewa. Utworzono nowe.");
            }

        }

        public override string ToString()
        {
            return "MCTS2v2";
        }




    }
}
