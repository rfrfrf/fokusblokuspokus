using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    public class Hand
    {
        public List<Piece> HandPieces { get; set; }

        public Hand()
        {
            HandPieces = Pieces.GetAllPieces();
        }

        public Hand(List<Piece> handPieces)
        {
            HandPieces = handPieces;
        }

        public void Remove(Move move)
        {
            if (!HandPieces.Remove(move.Piece))
            {
                throw new InvalidOperationException("Piece not found in hand");
            }
        }

        public void Add(Move move)
        {
            HandPieces.Add(move.Piece);
        }

        public Hand Clone()
        {
            return new Hand(new List<Piece>(HandPieces));
        }
    }
}
