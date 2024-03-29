﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Blokus.Logic;
using Blokus.Logic.MCTS;

namespace TestProject1
{
    /// <summary>
    /// Summary description for SerializationTest
    /// </summary>
    [TestClass]
    public class SerializationTest
    {
        public SerializationTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

    /*    [TestMethod]
        public void RulesTest()
        {
            var gs = new GameState();
            gs.AddMove(new Move(592129));
            gs.SwapCurrentPlayer();
            gs.AddMove(new Move(197634));
            gs.SwapCurrentPlayer();
            var dict = new Dictionary<int, Move>();
            var moves = GameRules.GetMoves(gs);

            foreach (var move in moves)
            {
                if (dict.ContainsKey(move.SerializedMove))
                {
                    int i = 2;
                }
                else
                {
                    dict.Add(move.SerializedMove, move);
                }
            }
        }*/

        [TestMethod]
        public void MergeTest()
        {
      /*      var root1 = new Node() { VisitCount = 9, WinCount = 5 };
            var root2 = new Node() { VisitCount = 3, WinCount = 17 };

            root1.AddChild(1, new Node() { VisitCount = 2, WinCount = 1 });
            root1.AddChild(2, new Node());
            root1.AddChild(3, new Node());

            root2.AllMovesCount = 4;
            root2.AddChild(1, new Node() { VisitCount = 3, WinCount = 3 });

            root1[1].AddChild(7, new Node() { VisitCount = 55, WinCount = 0, AllMovesCount = 34 });
            root2[1].AddChild(7, new Node() { VisitCount = 155, WinCount = 10});


            TreeMerger.TreeMerger.Merge(root1, root2);

            Assert.AreEqual(4, root1.AllMovesCount);
            Assert.AreEqual(5, root1[1].VisitCount);
            Assert.AreEqual(4, root1[1].WinCount);
            Assert.AreEqual(12, root1.VisitCount);
            Assert.AreEqual(22, root1.WinCount);

            Assert.AreEqual(210, root1[1][7].VisitCount);
            Assert.AreEqual(10, root1[1][7].WinCount);
            Assert.AreEqual(34, root1[1][7].AllMovesCount);*/
        }

        [TestMethod]
        public void NodeTest()
        {
          /*  var root = new Node();
            var move = new Move(3);
            root.AddChild(move.SerializedMove, new Node() { VisitCount = 666, WinCount = 12 });
            move = new Move(1);
            root.AddChild(move.SerializedMove, new Node());
            root[1].AllMovesCount = 4;
            move = new Move(2);
            root[3].AddChild(move.SerializedMove, new Node() { VisitCount = 9, WinCount = 10 });
            root.AllMovesCount = 42;

            var ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, root);
            ms.Position = 0;
            var actual = (Node)bf.Deserialize(ms);


            Assert.AreEqual(12, actual[3].WinCount);
            Assert.AreEqual(666, actual[3].VisitCount);
            Assert.IsTrue(actual.Children.Count == 2);
            Assert.IsTrue(actual.Children.ContainsKey(3));
            Assert.IsTrue(actual.Children.ContainsKey(1));
            Assert.IsTrue(actual[3].Children.Count == 1);
            Assert.IsTrue(actual[3].Children.ContainsKey(2));
            Assert.AreEqual(10, actual[3][2].WinCount);
            Assert.AreEqual(9, actual[3][2].VisitCount);

            Assert.AreEqual(42, actual.AllMovesCount);
            Assert.AreEqual(-1, actual[3].AllMovesCount);
            Assert.AreEqual(4, actual[1].AllMovesCount);
            Assert.AreEqual(-1, actual[3][2].AllMovesCount);
            Assert.IsNull(actual[1].Children);
            Assert.IsNull(actual[3][2].Children);*/
        }

        [TestMethod]
        public void MoveTest()
        {
            var move = new Move( Pieces.Pentamino05, new PiecePosition(6, 7), 3);
            var ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, move);
            ms.Position = 0;
            var actual = (Move)bf.Deserialize(ms);
            Assert.AreEqual(move, actual);


            move = new Move( Pieces.Tetramino05, new PiecePosition(9, 0),1);
            ms = new MemoryStream();
            bf = new BinaryFormatter();
            bf.Serialize(ms, move);
            ms.Position = 0;
            actual = (Move)bf.Deserialize(ms);
            Assert.AreEqual(move, actual);


            move = new Move(3);
            ms = new MemoryStream();
            bf = new BinaryFormatter();
            bf.Serialize(ms, move);
            ms.Position = 0;
            actual = (Move)bf.Deserialize(ms);
            Assert.AreEqual(move, actual);


            move = new Move(1);
            actual = new Move(Pieces.Monomino01,new PiecePosition(0, 0), 0);
            Assert.AreEqual(move, actual);



            int size = 4096;
            var moves = new Move[size];
            for (int i = 0; i < size; i++)
            {
                moves[i] = new Move(1 + i %3);
            }
            ms = new MemoryStream();
            bf = new BinaryFormatter();
            bf.Serialize(ms, moves);
            ms.Position = 0;
            var actuals = (Move[])bf.Deserialize(ms);
            Assert.AreEqual(moves.Length, actuals.Length);
            for (int i = 0; i < actuals.Length; i++)
            {
                Assert.AreEqual(moves[i], actuals[i]);
            }


            var movesList = new List<Move>(size);
            for (int i = 0; i < size; i++)
            {
                movesList.Add(new Move(1 + i % 3));
            }
            ms = new MemoryStream();
            bf = new BinaryFormatter();
            bf.Serialize(ms, movesList);
            ms.Position = 0;
            var actualsList = (List<Move>)bf.Deserialize(ms);
            Assert.AreEqual(movesList.Count, actualsList.Count);
            for (int i = 0; i < actualsList.Count; i++)
            {
                Assert.AreEqual(movesList[i], actualsList[i]);
            }

            Assert.AreEqual(new Move(3), new Move(3));
            Assert.AreEqual(new Move(Pieces.Pentamino06, new PiecePosition(6, 7), 1), new Move(Pieces.Pentamino06, new PiecePosition(6, 7), 1));
            Assert.AreNotEqual(new Move(3), new Move(2));

            Assert.IsTrue(new Move(3) == new Move(3));
            Assert.IsTrue(new Move(Pieces.Pentamino06, new PiecePosition(6, 7), 1) == new Move(Pieces.Pentamino06, new PiecePosition(6, 7), 1));
            Assert.IsFalse(new Move(4) == new Move(3));
            Assert.IsFalse(new Move(Pieces.Pentamino06, new PiecePosition(7, 6), 1) == new Move(Pieces.Pentamino06, new PiecePosition(6, 7), 1));

            Assert.IsFalse(new Move(3) != new Move(3));
            Assert.IsFalse(new Move(Pieces.Pentamino06, new PiecePosition(6, 7), 1) != new Move(Pieces.Pentamino06, new PiecePosition(6, 7), 1));
            Assert.IsTrue(new Move(4) != new Move(3));
            Assert.IsTrue(new Move(Pieces.Pentamino06, new PiecePosition(7, 6), 1) != new Move(Pieces.Pentamino06, new PiecePosition(6, 7), 1));

            Assert.IsTrue(new Move(3) != null);
            Assert.IsTrue(null != new Move(3));
            Move movenull = null;
            Assert.IsFalse(movenull != null);
            Assert.IsFalse(null != movenull);
            Assert.IsTrue(movenull == null);
            Assert.IsTrue(null == movenull);
            Assert.AreNotEqual(movenull, new Move(1));
            Assert.AreEqual(movenull, null);
            Assert.AreNotEqual(new Move(1), movenull);
            Assert.AreEqual(null, movenull);
        }
    }
}
