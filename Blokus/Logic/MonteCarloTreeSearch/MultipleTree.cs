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
        private const double Cvalue = 1;
        private const double Wvalue = 100;
        private static Random r = new Random();
        private static MultipleTreeNode root = null;
        Heuristics heuristic = new AlphaBeta.AlphaBetaHeuristics();

        private Player mePlayer;

        private static MultipleTreeNode currentNode;
        private int MovesCountWhenOpponentBlocked;// = int.MaxValue;//ilość ruchów po których przeciwnik się zablokował
        //private static int AllMadeMoves;//sprawdzanie czy nastąpił ruch
        private static bool isOpponentBlocked;//czy przeciwnik się zablokował
        private bool playsWithMCTS;
        private static bool przesuniecieWDrzewie;//gdy zaczyna jako drugi z nie-MCTS
        private static bool wczytaneZPliku;//gdy wczytano z pliku

        public MultipleTree()
        {
            //root = new MultipleTreeNode();
            currentNode = null;// new MultipleTreeNode();
            MovesCountWhenOpponentBlocked = int.MaxValue;
            isOpponentBlocked = false;
            playsWithMCTS = true;
            przesuniecieWDrzewie = false;
            //////////////////////////////////
            //wczytaneZPliku = false;
            /////////////////////////////////
            //GameState gs = new GameState();
            //root.childrenList = GameRules.GetMoves(gs);
        }


        public static void SaveTree(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, root);
            fs.Close();
        }

        public static void ReadTree(string filename)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                root = (MultipleTreeNode)bf.Deserialize(fs);
                wczytaneZPliku = true;
                fs.Close();
            }
            catch (Exception)
            {
                root = new MultipleTreeNode();
                wczytaneZPliku = false;
            }
            
        }

        public Move MakeMove(GameState gs)
        {
            //AllMadeMoves++;// = prevMoves.Count;
            List<Move> allavMoves = GameRules.GetMoves(gs);
            if (allavMoves.Count == 0)//gracz zablokowany
            {
                isOpponentBlocked = true;
                return null;
            }

            List<Move> prevMoves = gs.AllMoves;
            if (currentNode == null)
            {
                currentNode = root;
            }
            //if (prevMoves.Count == 1 && !playsWithMCTS)// && !root.childrenList.Exists(e=>e.move==prevMoves.ElementAt(0)))
            //{
            //    MultipleTreeNode pomroot = new MultipleTreeNode();
            //    pomroot.childrenList.Add(root);
            //    root.move = prevMoves.ElementAt(0);
            //    root.parentNode = pomroot;
            //    root = pomroot;
            //}




            GameState pomgs = PrepareGameState(currentNode);
            //pomgs.SwapCurrentPlayer();
            List<Move> someMoves = GameRules.GetMoves(pomgs);
            if (prevMoves.Count > 0 && someMoves.Exists(e => e.Equals(prevMoves.ElementAt(prevMoves.Count - 1))))//czyli każdy algorytm ma swoje osobne drzewo LUB gra z nie-MCTS
            {
                playsWithMCTS = false;
                if (prevMoves.Count == 1 && !playsWithMCTS && !przesuniecieWDrzewie && !wczytaneZPliku)// && !root.childrenList.Exists(e=>e.move==prevMoves.ElementAt(0)))
                {
                    MultipleTreeNode pomroot = new MultipleTreeNode();
                    pomroot.childrenList.Add(root);
                    root.move = prevMoves.ElementAt(0);
                    root.parentNode = pomroot;
                    root = pomroot;
                    przesuniecieWDrzewie = true;
                }
                if (!currentNode.childrenList.Exists(e => e.move.Equals(prevMoves.ElementAt(prevMoves.Count - 1))))//currentNode nie ma odpowiedniego dziecka
                {
                    MultipleTreeNode chpom = new MultipleTreeNode(prevMoves.ElementAt(prevMoves.Count - 1), currentNode);
                    currentNode.childrenList.Add(chpom);
                    currentNode = chpom;
                }
                else
                {
                    currentNode = currentNode.childrenList.Find(e => e.move.Equals(prevMoves.ElementAt(prevMoves.Count - 1)));
                }
                
            }
            else
            {
                if ((prevMoves.Count > 1 && MovesCountWhenOpponentBlocked == int.MaxValue && !playsWithMCTS) || (prevMoves.Count > 1 && playsWithMCTS && isOpponentBlocked && MovesCountWhenOpponentBlocked == int.MaxValue))// && AllMadeMoves!=prevMoves.Count)
                {
                    MovesCountWhenOpponentBlocked = prevMoves.Count;
                }
                //GameState pomgs2 = PrepareGameState(currentNode);
                //pomgs2.SwapCurrentPlayer();
                //List<Move> someMoves2 = GameRules.GetMoves(pomgs2);
            }
            
            
            //if(prevMoves.Count>0 && )



            //w tym miejscu currentNode na właściwym miejscu


            int pomchildrencount = currentNode.childrenList.Count;
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
                currentNode.childrenList.Sort((a, b) => { return b.victoryCount - a.victoryCount; });

                currentNode = currentNode.childrenList.ElementAt(0);
                resMove = currentNode.move;
            }
            


            if (resMove != null && !allavMoves.Exists(e => e.Equals(resMove)))
            {
                int a = 0;
                a++;

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
                    if (node.childrenList!=null && (pomNode = node.childrenList.Find(e => e.move.Equals(m))) != null)
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

            if (isGlobalMaxFromTree)
            {
                gs.AddMove(globalMaxMove);
                gs.SwapCurrentPlayer();
                SelectNode(node.childrenList.Find(e => e.move.Equals(globalMaxMove)), out maxNode, out maxNodeFormula, gs);
                gs.SwapCurrentPlayer();
                gs.DelMove(globalMaxMove);
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
            
            while (block_counter < 2)
            {
                heuristic.SortHand(gs);
                List<Move> moves = GameRules.GetMoves(gs, 2+(int)(Math.Pow(r.NextDouble(), 2.0) * 1666)); //uzycie randa promuje ruchy z poczatku listy
                if (moves.Count > 0)
                {
                    heuristic.SortMoves(gs, moves);

                    var m = GetBestMove(gs, moves);
                    
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

        /// <summary>
        /// wybiera 4 losowe ruchy mniej wiecej z poczatku listy moves a nastepnie wybiera z tych 4 ten, co daje 
        /// najgorsza ocene planszy przeciwinkowi
        /// </summary>
        private Move GetBestMove(GameState gs, List<Move> moves)
        {
            int[] indices = new int[4];
            double maxeval = double.NegativeInfinity;
            int maxi = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = Math.Min(moves.Count - 1, 10 + 2 * (int)(Math.Pow(r.NextDouble(), 2.0) * moves.Count));
            }
            for (int i = 0; i < indices.Length; i++)
            {
                double eval;
                gs.AddMove(moves[indices[i]]);
                eval = -heuristic.GetBoardEvaluation(gs);
                if (eval > maxeval)
                {
                    maxeval = eval;
                    maxi = i;
                }
                gs.DelMove(moves[indices[i]]);
            }
            return moves.ElementAt(maxi);
        }

        public void Backpropagate(int result, MultipleTreeNode node)
        {
            MultipleTreeNode pom=node;
            do
            {
                pom.victoryCount += result;
                pom.visitCount++;
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
