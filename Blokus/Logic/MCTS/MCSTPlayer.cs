using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.AlphaBeta;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Blokus.Logic.MonteCarloTreeSearch;
using System.Windows;

namespace Blokus.Logic.MCTS
{
    [Serializable]
    public class Node : ISerializable
    {
        public int VisitCount = 0;
        public int WinCount = 0;
        public Dictionary<int, Node> Children = new Dictionary<int, Node>();

        public bool IsLeaf { get { return Children == null || Children.Count == 0; } }

        public Node this[int i]
        {
            get
            {
                if (Children != null && Children.ContainsKey(i))
                {
                    return Children[i];
                }
                else
                {
                    return null;
                }
            }
        }

        public Node() { }

        protected Node(SerializationInfo info, StreamingContext context)
        {
            VisitCount = info.GetInt32("v");
            WinCount = info.GetInt32("w");
            Children = (Dictionary<int, Node>)info.GetValue("d", typeof(Dictionary<int, Node>));
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("v", VisitCount);
            info.AddValue("w", WinCount);
            info.AddValue("d", Children);
        }

        #endregion
    }

    public class MCSTPlayer : AIPlayer
    {
        private static Node _Root;

        private AlphaBetaPlayer _Playouter = new AlphaBetaPlayer();
        private Random _Random = new Random();
        private Node _CurrentNode;
        private Player _MyColor;
        private Move _LastMove;
        private Heuristics _Heursitics = new MCSTHeuristics();
        private bool _Training = false;

        public MCSTPlayer()
        {
            if (_Root == null)
            {
                _Root = new Node();
            }
        }

        public int MaxDepth
        {
            get { return _Playouter.MaxDepth; }
            set { _Playouter.MaxDepth = value; }
        }

        public int MaxTreeRank
        {
            get { return _Playouter.MaxTreeRank; }
            set { _Playouter.MaxTreeRank = value; }
        }

        public override void OnGameStart(GameState gameState)
        {
            _CurrentNode = _Root;
            _Training = gameState.VioletPlayer is MCSTPlayer && gameState.OrangePlayer is MCSTPlayer;
        }

        public override Move GetMove(GameState gameState)
        {
            if (_Training)
            {
                if (gameState.CurrentPlayerColor == Player.Orange)
                {
                    uctSearch(gameState, _Root);
                }
                return null;
            }

            if (gameState.AllMoves.Count > 0) // posun currentNode o ruch przeciwnika
            {
                if (_CurrentNode != null)
                {
                    var parent = _CurrentNode;
                    var lastMove = gameState.AllMoves[gameState.AllMoves.Count - 1];

                    _CurrentNode = _CurrentNode[lastMove.SerializedMove];
                    if (_CurrentNode == null) //dodaj do drzewa ruch przeciwnika
                    {
                        var node = new Node();
                        /*  if (lastMove.SerializedMove == 33557264)
                          {
                              int a = 3;
                          }*/
                        parent.Children.Add(lastMove.SerializedMove, node);
                    }
                }
            }
            _MyColor = gameState.CurrentPlayerColor;
            _LastMove = null;
            Mcst(gameState, _CurrentNode);
            if (_CurrentNode != null && _LastMove != null) // posun currentNode o nasz ruch
            {
                _CurrentNode = _CurrentNode[_LastMove.SerializedMove];
            }
            return _LastMove;
        }

        public override void OnGameEnd(GameState gameState)
        {
            if (!_Training)
            {
                int result = GameRules.GetWinner(gameState) == Player.Orange ? 1 : 0;

                var node = _Root;

                foreach (var move in gameState.AllMoves)
                {
                    node.VisitCount++;
                    node.WinCount += result;
                    node = node[move.SerializedMove];
                    if (node == null)
                    {
                        break;
                    }
                }
            }
        }

        private void Mcst(GameState gameState, Node node)
        {
            _Heursitics.SortHand(gameState); //wymieszaj klocki

            List<Move> moves;
            IEnumerable<Move> nodeChildren = null;
            if (node != null && !node.IsLeaf) //uzyj znanych ruchow z drzewa
            {
                nodeChildren = from c in node.Children select new Move(c.Key);

                /*     var set = new HashSet<Move>(GameRules.GetMoves(gameState));
                     foreach (var m in nodeChildren)
                     {
                         if (!set.Contains(m))
                         {
                             throw new InvalidOperationException("w drzewie jest ruch spoza drzewa gry");
                         }
                     }*/
            }
            else //wyszlismy poza drzewo, zwroc ruch z innego algorytmu
            {
                _LastMove = _Playouter.GetMove(gameState);
                return;
            }

            moves = GameRules.GetMoves(gameState, MaxTreeRank); //pobierz MaxTreeRank pierwszych dostepnych ruchow

            if (moves.Count == 0)
            {
                return; // pozycja koncowa
            }

            if (nodeChildren != null)
            {
                moves.AddRange(nodeChildren);
            }

            //   _Heursitics.SortMoves(gameState, moves); //posortuj ruchy by najlepsze byly na poczatku

            Move bestMove = null;
            double maxEval = double.NegativeInfinity;

            foreach (var move in moves)
            {
                double eval = EvaluateMove(move, gameState, node);
                if (eval > maxEval)
                {
                    bestMove = move;
                    maxEval = eval;
                }
            }
            if (node != null)
            {
                if (!node.Children.ContainsKey(bestMove.SerializedMove)) //wychodzimy za drzewo
                {
                    var newNode = new Node();
                    node.Children.Add(bestMove.SerializedMove, newNode);
                    _CurrentNode = null; //przestan sie posuwac w drzewie, by dodac tylko jeden wierzcholek na partie
                }
            }

            _LastMove = bestMove;
        }

        private double EvaluateMove(Move move, GameState gameState, Node node)
        {
            Node child;
            double result;
            if (node != null && (child = node[move.SerializedMove]) != null) //wierzcholek jest w drzewie, uzyj wiedzy o wincount
            {
                if (_MyColor == Player.Orange)
                {
                    result = (node.WinCount + 0.01) / (node.VisitCount + 0.1);
                }
                else
                {
                    result = ((node.VisitCount - node.WinCount) + 0.01) / (node.VisitCount + 0.1);
                }
            }
            else // wierzcholka nie ma w drzewie, uzyj heurystycznej oceny
            {
                gameState.AddMove(move);
                gameState.SwapCurrentPlayer();
                result = 0.5 - _Heursitics.GetBoardEvaluation(gameState);
                gameState.SwapCurrentPlayer();
                gameState.DelMove(move);
            }

            double add = _Random.NextDouble();
            add *= add;
            add *= add;
            result += add * add; //lekka nutka niedeterminizmu

            return result;
        }

        //tryb treningu - UCT

        private double _C = 1.0;

        private KeyValuePair<int, Node>? utcSelect(Node node)
        {
            var bestutc = 0.0;
            KeyValuePair<int, Node>? bestnode = null;

            foreach (var n in node.Children)
            {
                double utcvalue;
                if (n.Value.VisitCount == 0)
                {
                    // play unvisited nodes first
                    utcvalue = 10000 + _Random.Next(1000);
                }
                else
                {
                    double winrate = n.Value.WinCount / n.Value.VisitCount;
                    double utc = _C * Math.Sqrt(Math.Log(node.VisitCount, Math.E) / n.Value.VisitCount);
                    utcvalue = winrate + utc;
                }
                if (utcvalue > bestutc)
                {
                    bestutc = utcvalue;
                    bestnode = n;

                }
            }
            return bestnode;
        }

        private int playSimulation(GameState state, Node node)
        {
            int result = 0;
            if (node.VisitCount == 0)
            {
                result = playrandomgame(state);
            }
            else
            {
                if (node.IsLeaf)
                {
                    var moves = GameRules.GetMoves(state);
                    if (moves.Count == 0)
                    {
                        result = GameRules.GetWinner(state) == Player.Orange ? 1 : 0;
                        node.VisitCount += 1;
                        node.WinCount += result;

                        return result;
                    }
                    foreach (var move in moves)
                    {
                        node.Children.Add(move.SerializedMove, new Node());
                    }
                }

                var next = utcSelect(node);
                state.AddMove(new Move(next.Value.Key));
                state.SwapCurrentPlayer();
                result = playSimulation(state, next.Value.Value);
            }
            node.VisitCount += 1;
            node.WinCount += result;

            return result;
        }

        private Move uctSearch(GameState state, Node root)
        {

            playSimulation(state, root);

            return null;
        }

        private int playrandomgame(GameState state)
        {
            while (true)
            {
                var moves = GameRules.GetMoves(state);
                if (moves.Count == 0)
                {
                    break;
                }
                var move = moves[_Random.Next(moves.Count)];
                state.AddMove(move);
                state.SwapCurrentPlayer();
            }
            return GameRules.GetWinner(state)==Player.Orange ? 1:0;
        }


           private double Select(Node root, out Node bestNode)
           {
               var ln = Math.Log10(root.VisitCount + 1.0);
               var bestEval = double.NegativeInfinity;
               bestNode = null;

               foreach (var child in root.Children)
               {
                   var invN = 1.0 / (child.Value.VisitCount + 1.0);
                   var eval = child.Value.WinCount * invN + _C * Math.Sqrt(ln*invN);
                   if (eval > bestEval)
                   {
                       bestEval = eval;
                       bestNode = child.Value;
                   }
               }
               foreach (var child in root.Children)
               {
                   Node newNode;
                   var eval = Select(child.Value, out newNode);
                   if (eval > bestEval)
                   {
                       bestEval = eval;
                       bestNode = newNode;
                   }
               }
               return bestEval;
           }      



        public override string ToString()
        {
            return "MCTS2";
        }


        public static void SaveTree(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, _Root);
            fs.Close();
        }

        public static void ReadTree(string filename)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                _Root = (Node)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
                _Root = new Node();
                MessageBox.Show("Nie udało się wczytać drzewa. Utworzono nowe.");
            }

        }
    }
}
