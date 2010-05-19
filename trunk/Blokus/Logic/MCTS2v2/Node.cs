using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.MCTS2v2
{
    [Serializable]
    public class Node : ISerializable
    {
        public int VisitCount = 0;
        public int WinCount = 0;
        public Dictionary<int, Node> Children;
        public int AllMovesCount = -1;
        public int value=0;

        public bool IsLeaf { get { return Children == null || Children.Count == 0; } }

        public Node this[int i]
        {
            get
            {
                if (Children != null && Children.ContainsKey(i))
                {
                    return Children[i];
                }
                else
                {
                    return null;
                }
            }
        }

        public Node() { }

        protected Node(SerializationInfo info, StreamingContext context)
        {
            VisitCount = info.GetInt32("v");
            WinCount = info.GetInt32("w");
            Children = (Dictionary<int, Node>)info.GetValue("d", typeof(Dictionary<int, Node>));
            AllMovesCount = info.GetInt32("a");
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("v", VisitCount);
            info.AddValue("w", WinCount);
            info.AddValue("d", Children);
            info.AddValue("a", AllMovesCount);
        }

        #endregion

        public void AddChild(int move, Node node)
        {
            if (Children == null)
            {
                Children = new Dictionary<int, Node>();
            }
            Children.Add(move, node);
        }

        public void computeAverage(int R)
        {
            return;
        }
}
