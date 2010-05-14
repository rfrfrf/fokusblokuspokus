using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.MonteCarloTreeSearch
{
    [Serializable]
    public class MultipleTreeNode
    {
        public int visitCount = 0;
        public int victoryCount = 0;
        public Move move;

        public List<MultipleTreeNode> childrenList = null;
        public MultipleTreeNode parentNode = null;

        public MultipleTreeNode()
        {
            visitCount = 0;
            victoryCount = 0;
            move = null;
            parentNode = null;
            childrenList = null;
        }

        public MultipleTreeNode(Move _move, MultipleTreeNode _parentNode)
        {
            visitCount = 0;
            victoryCount = 0;
            move = _move;
            parentNode = _parentNode;
            childrenList = null;
        }

        public void ComputeAverage(int r)
        {
            //TODO: zrobic przeliczanie wartosci wezla
            return;
        }

        public bool IsLeaf()
        {
            return childrenList==null || childrenList.Count==0;
        }
    }
}
