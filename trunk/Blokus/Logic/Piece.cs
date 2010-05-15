using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Blokus.Logic
{
    [Serializable]
    public class Piece
    {
        public int Id { get; set; }
        public PieceVariant[] Variants { get; set; }

        public Piece() { }

        public Piece(PiecePosition[] squares, PieceOrientations orientations, int id)
        {
            Id = id;
            var variants = new List<PieceVariant>();

            if (IsSet(PieceOrientations.Rot0, orientations))
            {
                var variant = new PieceVariant(squares);
                variants.Add(variant);
            }
            if (IsSet(PieceOrientations.Rot90, orientations))
            {
                var variant = new PieceVariant(squares);
                variant.Rotate(90);
                variants.Add(variant);
            }
            if (IsSet(PieceOrientations.Rot180, orientations))
            {
                var variant = new PieceVariant(squares);
                variant.Rotate(180);
                variants.Add(variant);
            }
            if (IsSet(PieceOrientations.Rot270, orientations))
            {
                var variant = new PieceVariant(squares);
                variant.Rotate(270);
                variants.Add(variant);
            }

            if (IsSet(PieceOrientations.FlipRot0, orientations))
            {
                var variant = new PieceVariant(squares);
                variant.Flip();
                variants.Add(variant);
            }
            if (IsSet(PieceOrientations.FlipRot90, orientations))
            {
                var variant = new PieceVariant(squares);
                variant.Flip();
                variant.Rotate(90);
                variants.Add(variant);
            }
            if (IsSet(PieceOrientations.FlipRot180, orientations))
            {
                var variant = new PieceVariant(squares);
                variant.Flip();
                variant.Rotate(180);
                variants.Add(variant);
            }
            if (IsSet(PieceOrientations.FlipRot270, orientations))
            {
                var variant = new PieceVariant(squares);
                variant.Flip();
                variant.Rotate(270);
                variants.Add(variant);
            }

            Variants = variants.ToArray();
        }

        private bool IsSet(PieceOrientations flag, PieceOrientations orientations)
        {
            return (flag & orientations) == flag;
        }

        public override bool Equals(object obj)
        {
            var piece = obj as Piece;
            if (piece == null)
            {
                return false;
            }
            return piece.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }

    [Flags]
    public enum PieceOrientations
    {
        Rot0 = 1,
        Rot90 = 2,
        Rot180 = 4,
        Rot270 = 8,
        FlipRot0 = 16, //poziome odbicie lustrzane
        FlipRot90 = 32,
        FlipRot180 = 64,
        FlipRot270 = 128,
        Rotations = Rot0 | Rot90 | Rot180 | Rot270,
        All = Rotations | FlipRot0 | FlipRot90 | FlipRot180 | FlipRot270
    }

   
}
