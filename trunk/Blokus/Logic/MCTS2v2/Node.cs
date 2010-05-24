using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Blokus.Logic.MCTS2v2
{
    [Serializable]
    public class Node : ISerializable
    {
        public int VisitCount = 0;
        //public int WinCount = 0;
        public Dictionary<int, Node> Children;
        //public int AllMovesCount = -1;
        public int value = 0;
        //public Node parent;

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

        public Node()//Node _parent)
        {
            //parent = _parent;
            //Children = new Dictionary<int, Node>();
        }

        protected Node(SerializationInfo info, StreamingContext context)
        {
            VisitCount = info.GetInt32("v");
            //WinCount = info.GetInt32("w");
            Children = (Dictionary<int, Node>)info.GetValue("d", typeof(Dictionary<int, Node>));
            //AllMovesCount = info.GetInt32("a");
            value = info.GetInt32("w");
            //parent = (Node)info.GetValue("p", typeof(Node));
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("v", VisitCount);
            //info.AddValue("w", WinCount);
            info.AddValue("d", Children);
            //info.AddValue("a", AllMovesCount);
            info.AddValue("w", value);
            //info.AddValue("p", parent);
        }

        #endregion

        public void AddChild(int move, Node node)
        {
            if (Children == null)
            {
                Children = new Dictionary<int, Node>();
            }
            if (!Children.ContainsKey(move))
            {
                Children.Add(move, node);
                node.VisitCount++;
            }
        }

        public void computeAverage(int R)
        {
            if (R == int.MaxValue || R == int.MinValue)
            {
                this.value = R;
            }
            else
            {
                if ((R > 0 && value < int.MaxValue) || (R < 0 && value > int.MinValue))
                {
                    value += R;
                }
            }
        }
    }
}
