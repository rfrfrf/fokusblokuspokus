using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.MCTS;

namespace TreeMerger
{
    public class TreeMerger
    {
        public static void Merge(Node resultTree, Node node)
        {
            resultTree.VisitCount += node.VisitCount;
            resultTree.WinCount += node.WinCount;

            if (resultTree.AllMovesCount == -1)
            {
                resultTree.AllMovesCount = node.AllMovesCount;
            }
            else
            {
                if (node.AllMovesCount != -1 && node.AllMovesCount != resultTree.AllMovesCount)
                {
                    throw new InvalidOperationException("Liczba ruchów w węzłach powinna się zgadzać, jeśli jest ustawiona dla obydwu węzłów");
                }
            }
            if (!node.IsLeaf)
            {
                foreach (var child in node.Children)
                {
                    if (resultTree.IsLeaf || !resultTree.Children.ContainsKey(child.Key))
                    {
                        resultTree.AddChild(child.Key, child.Value);
                    }
                    else
                    {
                        Node pomchild = resultTree[child.Key];
                        Merge(pomchild, child.Value);
                    }
                }
            }
        }
    }
}
