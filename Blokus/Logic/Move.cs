using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Blokus.Logic
{
    [Serializable]
    public class Move
    {
        public Piece Piece { get; set; }
        public PiecePosition Position { get; set; }
        public int VariantNumber { get; set; }

        public PieceVariant PieceVariant
        {
            get { return Piece.Variants[VariantNumber]; }
        }

        public override bool Equals(object obj)
        {
            Move move = obj as Move;
            if (move == null)
            {
                return false;
            }
            return move.Piece.Equals(Piece) && move.Position.Equals(Position) && move.VariantNumber.Equals(VariantNumber);
        }

        public override int GetHashCode()
        {
            const int pieceCount = 22;
            unchecked
            {
                return Piece.GetHashCode() + pieceCount * Position.GetHashCode() + VariantNumber.GetHashCode();
            }
        }
    }
}
