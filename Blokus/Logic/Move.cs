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
        private Piece _Piece;
        private PiecePosition _Position;
        private int _VariantNumber;
        private int _SerializedMove;

        public Piece Piece
        {
            get { return _Piece; }
            set { _Piece = value; MoveToInt(); }
        }
        
        public PiecePosition Position
        {
            get { return _Position; }
            set { _Position = value; MoveToInt(); }
        }
        
        public int VariantNumber
        {
            get { return _VariantNumber; }
            set { _VariantNumber = value; MoveToInt(); }
        }
        
        public int SerializedMove
        {
            get { return _SerializedMove; }
            set { _SerializedMove = value; IntToMove(); }
        }

        public PieceVariant PieceVariant
        {
            get { return Piece.Variants[VariantNumber]; }
        }

        public override bool Equals(object obj)
        {
            Move move = obj as Move;
            if ( null == (object)move)
            {
                return false;
            }
            return move._SerializedMove == _SerializedMove;
        }

        public override int GetHashCode()
        {
            return _SerializedMove;
        }

        public static bool operator ==(Move obj, object obj2)
        {
            if (null == (object)obj2)
            {
                return null == (object)obj;
            }
            return obj2.Equals(obj);
        }

        public static bool operator !=(Move obj, object obj2)
        {
            if (null == (object)obj2)
            {
                return null != (object)obj;
            }
            return !obj2.Equals(obj);
        }

     //   public Move() { }

        public Move(int move)
        {
            _SerializedMove = move;
            IntToMove();
        }

        public Move(Piece piece, PiecePosition position, int variantNumber)
        {
            _Piece = piece;
            _Position = position;
            _VariantNumber = variantNumber;
            MoveToInt();
        }

        protected Move(SerializationInfo info, StreamingContext context)
        {
            SerializedMove = info.GetInt32("m");            
        }

        [SecurityPermissionAttribute(SecurityAction.Demand,
        SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {            
            info.AddValue("m", SerializedMove);            
        }

        private void MoveToInt()
        {
            byte i = (byte)Piece.Id;
            byte x = (byte)Position.X;
            byte y = (byte)Position.Y;
            byte v = (byte)VariantNumber;

            _SerializedMove = i | x << 8 | y << 16 | v << 24;
        }

        private void IntToMove()
        {
            int mask = 255;

            byte i = (byte)(mask & SerializedMove);
            mask <<= 8;
            byte x = (byte)((mask & SerializedMove)>>8);
            mask <<= 8;
            byte y = (byte)((mask & SerializedMove) >> 16);
            mask <<= 8;
            byte v = (byte)((mask & SerializedMove) >> 24);

            _Piece = Pieces.GetImmutablePieces()[i - 1];
            _Position = new PiecePosition(x,y);
            _VariantNumber = v;
        }
    }
}
