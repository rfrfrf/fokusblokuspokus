using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Blokus.Logic
{
    [Serializable]
    public class Move : ISerializable 
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

        public Move() { }

        protected Move(SerializationInfo info, StreamingContext context)
        {
            Piece = Pieces.GetImmutablePieces()[info.GetInt32("p")-1];
            Position = new PiecePosition(info.GetInt32("x"), info.GetInt32("y"));
            VariantNumber = info.GetInt32("v");
        }

        [SecurityPermissionAttribute(SecurityAction.Demand,
        SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("p", Piece.Id);            
            info.AddValue("x", Position.X);
            info.AddValue("y", Position.Y);
            info.AddValue("v", VariantNumber);
        }
    }
}
