using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.MonteCarloTreeSearch
{
    public class MultipleTree
    {
        public const double Cvalue = 1;
        MultipleTreeNode root = null;

        public MultipleTree()
        {
            root = new MultipleTreeNode();

            //GameState gs = new GameState();
            //root.childrenList = GameRules.GetMoves(gs);
        }


        public MultipleTreeNode Expansion(MultipleTreeNode selectedNode)
        {
            if (selectedNode.childrenList==null || selectedNode.childrenList.Count == 0)
            {
                List<Move> movelist = new List<Move>();
                MultipleTreeNode pomNode=selectedNode;
                do
                {
                    movelist.Add(pomNode.move);
                    pomNode = pomNode.parentNode;
                }
                while (pomNode != null);

                GameState gs = new GameState();
                for (int i = movelist.Count-1; i >= 0; i--)
                {
                    gs.AddMove(movelist.ElementAt(i));
                }

                selectedNode.childrenList = new List<MultipleTreeNode>();

                foreach (Move item in GameRules.GetMoves(gs))
                {
                    selectedNode.childrenList.Add(new MultipleTreeNode(item, selectedNode));
                }
            }
            
            return selectedNode.childrenList.ElementAt(0);
        }

        public void AddToTree(MultipleTreeNode treeNode, MultipleTreeNode nodeToAdd)
        {
            treeNode.childrenList.Add(nodeToAdd);
        }

        public void SelectNode(out MultipleTreeNode maxNode, out double maxNodeFormula)
        {
            SelectNode(root, out maxNode, out maxNodeFormula);
        }

        private void SelectNode(MultipleTreeNode node, out MultipleTreeNode maxNode, out double maxNodeFormula)
        {
            maxNodeFormula = CountNode(node);
            maxNode = node;
            double pomform;
            MultipleTreeNode pomNode;
            foreach (var item in node.childrenList)
            {
                SelectNode(item, out pomNode, out pomform);
                if (pomform > maxNodeFormula && pomNode.childrenList.Count!=0)
                {
                    maxNodeFormula = pomform;
                    maxNode = pomNode;
                }
            }
        }

        private double CountNode(MultipleTreeNode node)
        {
            if (node.parentNode != null)
            {
                return node.victoryCount + Cvalue * Math.Sqrt(Math.Log(node.parentNode.visitCount) / node.visitCount);
            }
            else
            {
                return node.victoryCount;
            }
        }
    }
}
