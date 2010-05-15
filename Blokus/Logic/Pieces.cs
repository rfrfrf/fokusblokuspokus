using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    [Serializable]
    public class Pieces
    {
        private static IList<Piece> cachedPieces;

        public static IList<Piece> GetImmutablePieces()
        {
            if (cachedPieces == null)
            {
                var result = new List<Piece>(){ 
                Monomino01, 
                Domino01, 
                Triomino01, Triomino02, 
                Tetramino01, Tetramino02, Tetramino03, Tetramino04, Tetramino05,
                Pentamino01, Pentamino02, Pentamino03, Pentamino04, Pentamino05, Pentamino06,
                Pentamino07, Pentamino08, Pentamino09, Pentamino10, Pentamino11, Pentamino12};

                /*    result.Sort((x, y) =>
                    {
                        int a = x.Variants[0].CornersIndicies.Length;
                        int b = y.Variants[0].CornersIndicies.Length;
                        if (a != b)
                        {
                            return b.CompareTo(a);
                        }
                        return y.Variants[0].Squares.Length.CompareTo(x.Variants[0].Squares.Length);
                    });*/
                cachedPieces = result.ToArray();
            }
            return cachedPieces;
        }

        public static List<Piece> GetAllPieces()
        {
            var result = new List<Piece>(){ 
                Monomino01, 
                Domino01, 
                Triomino01, Triomino02, 
                Tetramino01, Tetramino02, Tetramino03, Tetramino04, Tetramino05,
                Pentamino01, Pentamino02, Pentamino03, Pentamino04, Pentamino05, Pentamino06,
                Pentamino07, Pentamino08, Pentamino09, Pentamino10, Pentamino11, Pentamino12};

            /*    result.Sort((x, y) =>
                {
                    int a = x.Variants[0].CornersIndicies.Length;
                    int b = y.Variants[0].CornersIndicies.Length;
                    if (a != b)
                    {
                        return b.CompareTo(a);
                    }
                    return y.Variants[0].Squares.Length.CompareTo(x.Variants[0].Squares.Length);
                });*/

            return result;
        }

        //lista klockow
        //http://www.blokus.com//zoom.php?url=http://www.blokus.com//img/regles/bcf1.jpg
        //Poczatek ukladu pozycji klockow w lewym gornym rogu, os x skierowana w prawo, os y skierowana w dol.

        //*
        public static Piece Monomino01
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0) },
                    PieceOrientations.Rot0,1);
            }
        }

        //*
        //*
        public static Piece Domino01
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1) },
                    PieceOrientations.Rot0 | PieceOrientations.Rot90,2);
            }
        }

        //*
        //*
        //*
        public static Piece Triomino01
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), new PiecePosition(0, 2) },
                    PieceOrientations.Rot0 | PieceOrientations.Rot90,3);
            }
        }

        //*
        //**
        public static Piece Triomino02
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), new PiecePosition(1, 1) },
                    PieceOrientations.Rotations,4);
            }
        }

        //*
        //*
        //*
        //*
        public static Piece Tetramino01
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), new PiecePosition(0, 2), new PiecePosition(0, 3) },
                    PieceOrientations.Rot0 | PieceOrientations.Rot90,5);
            }
        }

        //*
        //*
        //**
        public static Piece Tetramino02
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), new PiecePosition(0, 2), new PiecePosition(1, 2) },
                    PieceOrientations.All,6);
            }
        }

        //*
        //**
        //*
        public static Piece Tetramino03
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), new PiecePosition(0, 2), new PiecePosition(1, 1) },
                    PieceOrientations.Rotations,7);
            }
        }

        //**
        //**        
        public static Piece Tetramino04
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), new PiecePosition(1, 0), new PiecePosition(1, 1) },
                    PieceOrientations.Rot0,8);
            }
        }

        //**
        // **        
        public static Piece Tetramino05
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(1, 0), new PiecePosition(1, 1), new PiecePosition(2, 1) },
                    PieceOrientations.Rot0 | PieceOrientations.Rot90 | PieceOrientations.FlipRot0 | PieceOrientations.FlipRot90,9);
            }
        }

        //*
        //*
        //*
        //*
        //*
        public static Piece Pentamino01
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(0, 2), new PiecePosition(0, 3), new PiecePosition(0, 4) },
                    PieceOrientations.Rot0 | PieceOrientations.Rot90,10);
            }
        }

        //*
        //*
        //*
        //**
        public static Piece Pentamino02
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(0, 2), new PiecePosition(0, 3), new PiecePosition(1, 3) },
                    PieceOrientations.All,11);
            }
        }

        //*
        //*
        //**
        // *
        public static Piece Pentamino03
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(0, 2), new PiecePosition(1, 2), new PiecePosition(1, 3) },
                    PieceOrientations.All,12);
            }
        }

        //*
        //**
        //**
        public static Piece Pentamino04
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(0, 2), new PiecePosition(1, 1), new PiecePosition(1, 2) },
                    PieceOrientations.All,13);
            }
        }

        //**
        //*
        //**
        public static Piece Pentamino05
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(0, 2), new PiecePosition(1, 0), new PiecePosition(1, 2) },
                    PieceOrientations.Rotations,14);
            }
        }

        //*
        //**
        //*
        //*
        public static Piece Pentamino06
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(0, 2), new PiecePosition(0, 3), new PiecePosition(1, 1) },
                    PieceOrientations.All,15);
            }
        }

        //*
        //***
        //*
        public static Piece Pentamino07
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(0, 2), new PiecePosition(1, 1), new PiecePosition(2, 1) },
                    PieceOrientations.Rotations,16);
            }
        }

        //*
        //*
        //***
        public static Piece Pentamino08
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(0, 2), new PiecePosition(1, 2), new PiecePosition(2, 2) },
                    PieceOrientations.Rotations,17);
            }
        }

        //**
        // **
        //  *
        public static Piece Pentamino09
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(1, 0), 
                    new PiecePosition(1, 1), new PiecePosition(2, 1), new PiecePosition(2, 2) },
                    PieceOrientations.Rotations,18);
            }
        }

        //*
        //***
        //  *
        public static Piece Pentamino10
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(1, 1), new PiecePosition(2, 1), new PiecePosition(2, 2) },
                    PieceOrientations.Rot0 | PieceOrientations.Rot90 | PieceOrientations.FlipRot0 | PieceOrientations.FlipRot90,19);
            }
        }

        //*
        //***
        // *
        public static Piece Pentamino11
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(0, 0), new PiecePosition(0, 1), 
                    new PiecePosition(1, 1), new PiecePosition(2, 1), new PiecePosition(1, 2) },
                    PieceOrientations.All,20);
            }
        }

        //*
        //***
        // *
        public static Piece Pentamino12
        {
            get
            {
                return new Piece(new PiecePosition[] { new PiecePosition(1, 0), new PiecePosition(0, 1), 
                    new PiecePosition(1, 1), new PiecePosition(2, 1), new PiecePosition(1, 2) },
                    PieceOrientations.Rot0,21);
            }
        }
    }
}
