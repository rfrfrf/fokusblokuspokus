using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Blokus.Logic;

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

        [TestMethod]
        public void MoveTest()
        {
            var move = new Move();
            move.Piece = Pieces.Pentamino05;
            move.Position = new PiecePosition(6, 7);
            move.VariantNumber = 3;
            var ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, move);
            ms.Position = 0;
            var actual = (Move)bf.Deserialize(ms);
            Assert.AreEqual(move, actual);

            move = new Move();
            move.Piece = Pieces.Tetramino05;
            move.Position = new PiecePosition(9, 0);
            move.VariantNumber = 1;
            ms = new MemoryStream();
            bf = new BinaryFormatter();
            bf.Serialize(ms, move);
            ms.Position = 0;
            actual = (Move)bf.Deserialize(ms);
            Assert.AreEqual(move, actual);
        }
    }
}
