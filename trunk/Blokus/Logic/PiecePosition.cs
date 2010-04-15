using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    public struct PiecePosition
    {
        public int X;
        public int Y;

        public PiecePosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public PiecePosition(PiecePosition position)
        {
            X = position.X;
            Y = position.Y;
        }

        public static PiecePosition operator +(PiecePosition p1, PiecePosition p2)
        {
            PiecePosition temp = new PiecePosition();
            temp.X = p1.X + p2.X;
            temp.Y = p1.Y + p2.Y;
            return temp;
        }

        public static PiecePosition operator -(PiecePosition p1, PiecePosition p2)
        {
            PiecePosition temp = new PiecePosition();
            temp.X = p1.X - p2.X;
            temp.Y = p1.Y - p2.Y;
            return temp;
        }

        public void Rotate(int angle)
        {
            double theta = angle * (Math.PI / 180.0);
            double x = Math.Cos(theta) * X - Math.Sin(theta) * Y;
            double y = Math.Sin(theta) * X + Math.Cos(theta) * Y;
            X = (int)Math.Round(x);
            Y = (int)Math.Round(y);
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ")";
        }
    }
}
