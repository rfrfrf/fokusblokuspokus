using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace Blokus.Logic.MonteCarloTreeSearch
{
    public class MultipleTree
    {
        public const double Cvalue = 1;
        public const double Wvalue = 100;
        public static Random r = new Random();
        public MultipleTreeNode root = null;

        public Player mePlayer;

        public MultipleTreeNode currentNode;
        public int MovesCountWhenOpponentBlocked = int.MaxValue;//ilość ruchów po których przeciwnik się zablokował

        public MultipleTree()
        {
            root = new MultipleTreeNode();
            currentNode = null;// new MultipleTreeNode();

            //GameState gs = new GameState();
            //root.childrenList = GameRules.GetMoves(gs);
        }


        public void SaveTree(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, root);
            fs.Close();
        }

        public void ReadTree(string filename)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                root = (MultipleTreeNode)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
            }
            
        }

        public Move MakeMove(GameState gs)
        {
            List<Move> allavMoves = GameRules.GetMoves(gs);
            if (allavMoves.Count == 0)//gracz zablokowany
            {
                return null;
            }

            List<Move> prevMoves = gs.AllMoves;
            if (currentNode == null)
            {
                currentNode = root;
            }
            if (prevMoves.Count == 1)// && !root.childrenList.Exists(e=>e.move==prevMoves.ElementAt(0)))
            {
                MultipleTreeNode pomroot = new MultipleTreeNode();
                pomroot.childrenList.Add(root);
                root.move = prevMoves.ElementAt(0);
                root.parentNode = pomroot;
                root = pomroot;
            }




            GameState pomgs = PrepareGameState(currentNode);
            //pomgs.SwapCurrentPlayer();
            List<Move> someMoves = GameRules.GetMoves(pomgs);
            if (prevMoves.Count > 0 && someMoves.Exists(e => e.Equals(prevMoves.ElementAt(prevMoves.Count - 1))))//czyli każdy algorytm ma swoje osobne drzewo
            {
                if (!currentNode.childrenList.Exists(e => e.move.Equals(prevMoves.ElementAt(prevMoves.Count - 1))))//currentNode nie ma odpowiedniego dziecka
                {
                    MultipleTreeNode chpom = new MultipleTreeNode(prevMoves.ElementAt(prevMoves.Count - 1), currentNode);
                    currentNode.childrenList.Add(chpom);
                    currentNode = chpom;
                }
                else
                {
                    currentNode = currentNode.childrenList.Find(e => e.move == prevMoves.ElementAt(prevMoves.Count - 1));
                }
            }
            else
            {
                if (prevMoves.Count > 1 && MovesCountWhenOpponentBlocked==int.MaxValue)
                {
                    MovesCountWhenOpponentBlocked = prevMoves.Count;
                }
                GameState pomgs2 = PrepareGameState(currentNode);
                pomgs2.SwapCurrentPlayer();
                List<Move> someMoves2 = GameRules.GetMoves(pomgs2);
            }

            
            //if(prevMoves.Count>0 && )



            //w tym miejscu currentNode na właściwym miejscu

            
           
            this.mePlayer = gs.CurrentPlayerColor;
            MultipleTreeNode pomNode;
            double pomval;
            SelectNodeFromSubTree(currentNode, out pomNode, out pomval, gs);
            int res = Playout(pomNode);
            Backpropagate(res, pomNode);

            Move resMove = null;
            if (currentNode.childrenList.Count > 0)
            {

                //currentNode.childrenList.Sort(new Comparison<MultipleTreeNode>(delegate(MultipleTreeNode e, MultipleTreeNode k) { return e.victoryCount - k.victoryCount; }));
                currentNode.childrenList.Sort((a, b) => { return a.victoryCount - b.victoryCount; });

                currentNode = currentNode.childrenList.ElementAt(0);
                resMove = currentNode.move;
            }
            


            if (resMove != null && !allavMoves.Exists(e => e.Equals(resMove)))
            {

                throw new ArgumentException("zły ruch");
            }




            return resMove;
        }

        //public MultipleTreeNode Expansion(MultipleTreeNode selectedNode)
        //{
        //    if (selectedNode.childrenList==null || selectedNode.childrenList.Count == 0)
        //    {
        //        GameState gs = PrepareGameState(selectedNode);

        //        selectedNode.childrenList = new List<MultipleTreeNode>();

        //        foreach (Move item in GameRules.GetMoves(gs))
        //        {
        //            selectedNode.childrenList.Add(new MultipleTreeNode(item, selectedNode));
        //        }
        //    }
            
        //    return selectedNode.childrenList.ElementAt(0);
        //}

        private GameState PrepareGameState(MultipleTreeNode node)
        {
            if (node.move == null)
            {
                return new GameState();
            }
            List<Move> movelist = new List<Move>();
            MultipleTreeNode pomNode = node;
            do
            {
                movelist.Add(pomNode.move);
                pomNode = pomNode.parentNode;
            }
            while (pomNode != null && pomNode.move!=null);

            GameState gs = new GameState();
            for (int i = movelist.Count - 1; i >= 0; i--)
            {
                gs.AddMove(movelist.ElementAt(i));
                if (movelist.Count - i < MovesCountWhenOpponentBlocked)
                {
                    gs.SwapCurrentPlayer();
                }
            }
            return gs;
        }

        public void AddToTree(MultipleTreeNode treeNode, MultipleTreeNode nodeToAdd)
        {
            treeNode.childrenList.Add(nodeToAdd);
        }

        public void SelectNodeFromSubTree(MultipleTreeNode startNode, out MultipleTreeNode maxNode, out double maxNodeFormula, GameState gs)
        {
            SelectNode(startNode, out maxNode, out maxNodeFormula, gs);
        }



        private void SelectNode(MultipleTreeNode node, out MultipleTreeNode maxNode, out double maxNodeFormula, GameState gs)
        {
            //GameState gs=new GameState();
            //if (node.move != null)
            //{
            //    gs = PrepareGameState(node);
            //}
            List<Move> moves = GameRules.GetMoves(gs);
            double globalMaxForm = double.NegativeInfinity;
            Move globalMaxMove = null;
            bool isGlobalMaxFromTree = false;
            if (moves.Count > 0)
            {
                MultipleTreeNode pomNode = null;
                double pomformval = double.NegativeInfinity;

                
                foreach (Move m in moves)
                {
                    if (node.childrenList!=null && (pomNode = node.childrenList.Find(e => e.move == m)) != null)
                    {
                        pomformval = CountNode(pomNode);
                        if (globalMaxForm < pomformval)
                        {
                            //nowy max
                            globalMaxForm = pomformval;
                            globalMaxMove = m;
                            isGlobalMaxFromTree = true;
                        }
                    }
                    else
                    {
                        pomNode = new MultipleTreeNode(m, node);
                        pomformval = CountNode(pomNode);
                        if (globalMaxForm < pomformval)
                        {
                            //nowy max
                            globalMaxForm = pomformval;
                            globalMaxMove = m;
                            isGlobalMaxFromTree = false;
                        }


                    }
                }
            }

            node.visitCount++;
            if (isGlobalMaxFromTree)
            {
                SelectNode(node.childrenList.Find(e => e.move == globalMaxMove), out maxNode, out maxNodeFormula, gs);
            }
            else
            {
                MultipleTreeNode nnode = new MultipleTreeNode(globalMaxMove, node);
                AddToTree(node, nnode);
                maxNode = nnode;
                maxNodeFormula = globalMaxForm;
            }
            return;




            //if (node.IsLeaf())
            //{
            //    maxNode = node;
            //    maxNodeFormula = CountNode(node);
            //    return;
            //}
            //maxNodeFormula = double.NegativeInfinity ;//CountNode(node);
            //maxNode = null;// node;
            //double pomform;
            //MultipleTreeNode pomNode;
            //foreach (var item in node.childrenList)
            //{
            //    SelectNode(item, out pomNode, out pomform);
            //    if (pomform > maxNodeFormula)// && pomNode.childrenList.Count!=0)
            //    {
            //        maxNodeFormula = pomform;
            //        maxNode = pomNode;
            //    }
            //}
        }

        public int Playout(MultipleTreeNode nnode)
        {
            GameState gs = PrepareGameState(nnode);
            //bool myMove = gs.CurrentPlayerColor == mePlayer;
            int block_counter=0;//sprawdzenie, czy obaj gracze nie mogą wykonywać już ruchów
            var heuristic = new AlphaBeta.AlphaBetaHeuristics();
            while (block_counter < 2)
            {
                heuristic.SortHand(gs);
                List<Move> moves = GameRules.GetMoves(gs, r.Next(888) + 2); //uzycie randa promuje ruchy z poczatku listy
                if (moves.Count > 0)
                {
                    heuristic.SortMoves(gs, moves);
                    Move m = moves.ElementAt(r.Next(r.Next(moves.Count+1))); //uzycie 2 randow promuje ruchy z poczatku
                    gs.AddMove(m);
                    block_counter=0;
                }
                else
                {
                    block_counter++;
                }
                gs.SwapCurrentPlayer();
            }
            return GameRules.GetWinner(gs) == mePlayer ? 1 : 0;
            
        }

        public void Backpropagate(int result, MultipleTreeNode node)
        {
            MultipleTreeNode pom=node;
            do
            {
                pom.victoryCount += result;
                pom = pom.parentNode;

            } while (pom.parentNode != null);

        }

        /// <summary>
        /// wylicza wartosc noda, w oparciu o UCT
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private double CountNode(MultipleTreeNode node)
        {
            //if (node.parentNode != null)
            //{
            //    return node.victoryCount + Cvalue * Math.Sqrt(Math.Log(node.parentNode.visitCount) / node.visitCount);
            //}
            //else
            //{
            //    return node.victoryCount;
            //}
            return r.NextDouble() * (node.victoryCount+1.0) / (node.visitCount+10.0); 
            //ocena 0.1 dla nieodweidzonych, 1.0 dla liczby zwyciestw dazaczecj do nieskonczonosci
        }
    }
}
