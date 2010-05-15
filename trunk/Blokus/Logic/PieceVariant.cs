using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    [Serializable]
    public class PieceVariant
    {
        public PiecePosition[] Squares { get; private set; }

        private int[] cornersIndicies;

        public int[] CornersIndicies
        {
            get 
            { 
                return cornersIndicies == null ? (cornersIndicies = GetCornersIndicies()) : cornersIndicies; 
            }
            set 
            { 
                cornersIndicies = value; 
            }
        }

        private int[] allIndicies;

        public int[] AllIndicies
        {
            get
            {
                return allIndicies == null ? (allIndicies = GetAllIndicies()) : allIndicies;
            }
            set
            {
                allIndicies = value;
            }
        }

        public PieceVariant() { }

        public PieceVariant(IList<PiecePosition> squares)
        {
            Squares = new PiecePosition[squares.Count];

            for (int i = 0; i < squares.Count; i++)
            {
                Squares[i] = new PiecePosition(squares[i]);
            }
        }

        public void Rotate(int angle)
        {
            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i].Rotate(angle);
            }

            MoveSquaresOrginToZero();
            CornersIndicies = null;
        }

        private void MoveSquaresOrginToZero()
        {
            int minx = int.MaxValue, miny = int.MaxValue;

            foreach (var pos in Squares)
            {
                minx = Math.Min(minx, pos.X);
                miny = Math.Min(miny, pos.Y);
            }

            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i].X -= minx;
                Squares[i].Y -= miny;
            }
        }

        public void Flip()
        {
            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i].X = -Squares[i].X;
            }

            MoveSquaresOrginToZero();
            CornersIndicies = null;
        }

        private int[] GetCornersIndicies()
        {
            List<int> corners = new List<int>(Squares.Length);

            for (int i = 0; i < Squares.Length; i++)
            {
                if (!((ExistsAtPosition(Squares[i].X + 1, Squares[i].Y) &&
                    ExistsAtPosition(Squares[i].X - 1, Squares[i].Y)) ||
                    (ExistsAtPosition(Squares[i].X, Squares[i].Y + 1) &&
                    ExistsAtPosition(Squares[i].X, Squares[i].Y - 1))))
                {
                    corners.Add(i);
                }
            }

            return corners.ToArray();
        }

        private bool ExistsAtPosition(int x, int y)
        {
            return (from s in Squares where s.X == x && s.Y == y select s).Count() != 0;
        }

        private int[] GetAllIndicies()
        {
            var indicies = new int[Squares.Length];
            for (int i = 0; i < indicies.Length; i++)
            {
                indicies[i] = i;
            }

            return indicies;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var pt in Squares)
            {
                sb.Append(pt.ToString());
                sb.Append(", ");
            }
            if (sb.Length >= 2)
            {
                sb.Remove(sb.Length - 2, 2);
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
