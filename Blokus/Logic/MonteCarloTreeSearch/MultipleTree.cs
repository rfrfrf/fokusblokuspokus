﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Blokus.Logic.MonteCarloTreeSearch
{
    public class MultipleTree
    {
        public const double Cvalue = 1;
        public const double Wvalue = 100;
        public Random r;
        public MultipleTreeNode root = null;

        public Player mePlayer;

        public MultipleTree()
        {
            root = new MultipleTreeNode();

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
            List<Move> movelist = new List<Move>();
            MultipleTreeNode pomNode = node;
            do
            {
                movelist.Add(pomNode.move);
                pomNode = pomNode.parentNode;
            }
            while (pomNode != null);

            GameState gs = new GameState();
            for (int i = movelist.Count - 1; i >= 0; i--)
            {
                gs.AddMove(movelist.ElementAt(i));
                gs.SwapCurrentPlayer();
            }
            return gs;
        }

        public void AddToTree(MultipleTreeNode treeNode, MultipleTreeNode nodeToAdd)
        {
            treeNode.childrenList.Add(nodeToAdd);
        }

        public void SelectNodeFromSubTree(MultipleTreeNode startNode, out MultipleTreeNode maxNode, out double maxNodeFormula)
        {
            SelectNode(startNode, out maxNode, out maxNodeFormula);
        }



        private void SelectNode(MultipleTreeNode node, out MultipleTreeNode maxNode, out double maxNodeFormula)
        {
            GameState gs = PrepareGameState(node);
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
                    if ((pomNode = node.childrenList.Find(e => e.move == m)) != null)
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
                SelectNode(node.childrenList.Find(e => e.move == globalMaxMove), out maxNode, out maxNodeFormula);
            }
            else
            {
                MultipleTreeNode nnode = new MultipleTreeNode(globalMaxMove, node);
                //AddToTree(node, nnode);
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
                List<Move> moves = GameRules.GetMoves(gs);
                if (moves.Count > 0)
                {
                    Move m = moves.ElementAt((int)r.NextDouble() * moves.Count);
                    gs.AddMove(m);
                    block_counter--;
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
            return r.NextDouble() * (node.victoryCount+1) / (node.visitCount+1);
        }
    }
}
