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
using Blokus.Logic.Scout;

namespace Blokus.Logic.MCTS
{
    [Serializable]
    public class Node : ISerializable
    {
        public int VisitCount = 0;
        public int WinCount = 0;
        public Dictionary<int, Node> Children;
      //  public int AllMovesCount = -1;

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
      //      AllMovesCount = info.GetInt32("a");
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("v", VisitCount);
            info.AddValue("w", WinCount);
            info.AddValue("d", Children);
   //         info.AddValue("a", AllMovesCount);
        }

        #endregion

        public void AddChild(int move, Node node)
        {
            if (Children == null)
            {
                Children = new Dictionary<int, Node>();
            }
            Children.Add(move, node);
        }
    }

    public class MCSTPlayer : AIPlayer
    {
        private static Node _Root;

        private ScoutPlayer _Playouter = new ScoutPlayer();
        private Random _Random = new Random();
        private Node _CurrentNode;
        private Player _MyColor;
        private Move _LastMove;
        private Heuristics _Heursitics = new ScoutHeuristics();
     //   private bool _Training = false;

        public MCSTPlayer()
        {
            if (_Root == null)
            {
                _Root = new Node() { VisitCount = 1 };
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
     /*       _Training = gameState.VioletPlayer is MCSTPlayer && gameState.OrangePlayer is MCSTPlayer;*/
            if (gameState.AllMoves.Count > 0)
            {
                throw new InvalidDataException("Nieprawidłowy gamestate");
            }
        }

        public override Move GetMove(GameState gameState)
        {
       /*     if (_Training)
            {
                if (gameState.CurrentPlayerColor == Player.Orange)
                {
                    if (gameState.AllMoves.Count > 0)
                    {
                        throw new InvalidDataException("Nieprawidłowy gamestate");
                    }
                  //  uctSearch(gameState, _Root);
                    Play(gameState, _Root);
                }
                return null;
            }*/

            if (gameState.AllMoves.Count > 0) // posun currentNode o ruch przeciwnika
            {
                if (_CurrentNode != null)
                {
                    var parent = _CurrentNode;
                    var lastMove = gameState.AllMoves[gameState.AllMoves.Count - 1];

                    _CurrentNode = _CurrentNode[lastMove.SerializedMove];
                    if (_CurrentNode == null) //dodaj do drzewa ruch przeciwnika
                    {
                        _CurrentNode = new Node();
                        parent.AddChild(lastMove.SerializedMove, _CurrentNode);
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
            /*if (!_Training)*/
            {
                var winner = GameRules.GetWinner(gameState);
                int result = 0;
                switch (winner)
                {
                    case Player.None: result = 0; break;
                    case Player.Orange: result = 1; break;
                    case Player.Violet: result = -1; break;
                }

                var node = _Root;

                foreach (var move in gameState.AllMoves)
                {
                    node.VisitCount++;
                    node.WinCount += result;
                    result *= -1;
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

            HashSet<Move> moves = new HashSet<Move>();
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
                if (node != null && _LastMove!=null)
                {
                    node.AddChild(_LastMove.SerializedMove, new Node());
                    _CurrentNode = null;
                }
                return;
            }

            int rank = Math.Max(40, MaxTreeRank);
            var moveslist = GameRules.GetMoves(gameState, rank); //pobierz MaxTreeRank pierwszych dostepnych ruchow
            
            if (moveslist.Count == 0)
            {
                return; // pozycja koncowa
            }

            _Heursitics.SortMoves(gameState, moveslist);
            int i=0;
            foreach (var n in moveslist)
            {
                moves.Add(n);
                i++;
                if (i > MaxTreeRank)
                {
                    break;
                }
            }

            if (nodeChildren != null)
            {
                foreach (var n in nodeChildren)
                {
                    if (!moves.Contains(n))
                    {
                        moves.Add(n);
                    }
                }
            }

            Move bestMove = null;
            double maxEval = double.NegativeInfinity;

            foreach (var move in moves)
            {
                double eval = -EvaluateMove(move, gameState, node);
                if (eval > maxEval)
                {
                    bestMove = move;
                    maxEval = eval;
                }
            }
            if (node != null)
            {
                if (node.Children==null || !node.Children.ContainsKey(bestMove.SerializedMove)) //wychodzimy za drzewo
                {
                    var newNode = new Node();
                    node.AddChild(bestMove.SerializedMove, newNode);
                    _CurrentNode = null; //przestan sie posuwac w drzewie, by dodac tylko jeden wierzcholek na partie
                }
            }

            _LastMove = bestMove;
        }

        private double EvaluateMove(Move move, GameState gameState, Node node)
        {
            Node child;
            double result;
            if (node != null && (child = node[move.SerializedMove]) != null && child.VisitCount>6) //wierzcholek jest w drzewie, uzyj wiedzy o wincount
            {
                result = child.WinCount;
             /*   if (_MyColor == Player.Orange)
                {
                    result = ((double)child.WinCount) / (child.VisitCount);
                }
                else
                {
                    result = ((double)(child.VisitCount - child.WinCount)) / (child.VisitCount);
                }*/
               // result = Math.Pow(result, Math.Log(child.VisitCount));
            }
            else // wierzcholka nie ma w drzewie, uzyj heurystycznej oceny
            {
               // return _Random.NextDouble()*0.5;
                gameState.AddMove(move);
                gameState.SwapCurrentPlayer();

                result =  _Heursitics.GetBoardEvaluation(gameState);

                gameState.SwapCurrentPlayer();
                gameState.DelMove(move);
            }

            double add = _Random.NextDouble()*0.5;
            add *= add;
            add *= add;
            add *= add;
            result *=  1.0 + add * add; //lekka nutka niedeterminizmu
            
            return result;
        }

        //tryb treningu - UCT

     /*   private double _C = 1.0;

        /// <summary>
        /// zwraca node lub najlepsze z dzieci node i jego ocene. 
        /// path - jak dojsc do wezla, dla roota jest puste
        /// </summary>
        private KeyValuePair<double, Node> Select(Node node, int parentVisitcount, List<int> path)
        {
            var bestutc = double.NegativeInfinity;
            Node bestnode = null;

            if (node.IsLeaf || node.Children.Count < node.AllMovesCount)
            {
                double invVisit = 1.0 / ((double)node.VisitCount);
                double winrate = node.WinCount * invVisit;
                double utc = _C * Math.Sqrt(Math.Log(parentVisitcount, Math.E) * invVisit);
                bestutc = winrate + utc;
                bestnode = node;
            }
            List<int> bestpath = null;

            if (!node.IsLeaf)
            {
                foreach (var n in node.Children)
                {
                    if (n.Value.VisitCount != 0 && n.Value.AllMovesCount != 0)
                    {
                        var localpath = new List<int>() { n.Key };

                        var val = Select(n.Value, node.VisitCount, localpath);
                        if (val.Key > bestutc)
                        {
                            bestutc = val.Key;
                            bestnode = val.Value;
                            bestpath = localpath;
                        }
                    }
                }
                if (bestpath != null)
                {
                    path.AddRange(bestpath);
                }
            }
            return new KeyValuePair<double,Node>(bestutc, bestnode);
        }

        private int Play(GameState state, Node node)
        {
            var path = new List<int>();
            KeyValuePair<double, Node> pair;
            if (_Root.Children != null && _Root.Children.Count == _Root.AllMovesCount) //najpeirw wszystkie dzieci roota sprawdz
            {
                pair = Select(node, int.MaxValue, path);
            }
            else
            {
                pair = new KeyValuePair<double, Node>(0, node);
            }
            int result;
            foreach (var move in path)
            {
                state.AddMove(new Move(move));
                state.SwapCurrentPlayer();
            }
            var expandedNode = pair.Value;
            if (expandedNode == null)
            {
                throw new InvalidDataException("Przejrzałeś całe drzewo gry (podejrzane)");
            }
            var moves = GameRules.GetMoves(state);
            int newMove;
            if (expandedNode.IsLeaf)
            {
                expandedNode.AllMovesCount = moves.Count;
                if (moves.Count == 0)
                {
                    result = GameRules.GetWinner(state) == Player.Orange ? 1 : 0;
                    UpdatePath(node, path, result);
                    return result;
                }
                newMove = moves[_Random.Next(moves.Count)].SerializedMove;
                expandedNode.AddChild(newMove, new Node());
            }
            else
            {
                int i = _Random.Next(moves.Count);
                while (expandedNode.Children.ContainsKey(moves[i].SerializedMove))
                {
                    i++;
                    if (i >= moves.Count)
                    {
                        i = 0;
                    }
                }
                newMove = moves[i].SerializedMove;
                expandedNode.AddChild(newMove, new Node());
            }
            path.Add(newMove);
            state.AddMove(new Move(newMove));
            state.SwapCurrentPlayer();
            result = playrandomgame(state);

            UpdatePath(node, path, result);
            return result;
        }

        private void UpdatePath(Node node, List<int> path, int result)
        {
            foreach (var move in path)
            {
                node.VisitCount++;
                node.WinCount += result;
                node = node[move];
            }
            node.VisitCount++;
            node.WinCount += result;
        }

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
            List<Move> moves = null;
            if (node.VisitCount == 0)
            {
                result = playrandomgame(state);
            }
            else
            {
                if (node.IsLeaf || (_Random.Next(2)==0 && node.Children.Count != node.AllMovesCount))
                {
                    moves = GameRules.GetMoves(state);
                    node.AllMovesCount = moves.Count;
                    if (moves.Count == 0)
                    {
                        result = GameRules.GetWinner(state) == Player.Orange ? 1 : 0;
                        node.VisitCount += 1;
                        node.WinCount += result;                        
                        return result;
                    }
                }
                KeyValuePair<int, Node>? next;
                if (moves != null)
                {
                    int i = _Random.Next(moves.Count);
                    if(!node.IsLeaf)
                    {
                        while(node.Children.ContainsKey(moves[i].SerializedMove))
                        {
                            i++;
                            if(i>=moves.Count)
                            {
                                i=0;
                            }
                        }
                    }
                    var move = moves[i];
                    next = new KeyValuePair<int, Node>(move.SerializedMove, new Node());
                    node.AddChild(next.Value.Key, next.Value.Value);
                }
                else
                {
                    next = utcSelect(node);
                }
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
        }*/

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
                _Root = new Node() { VisitCount = 1 };
                MessageBox.Show("Nie udało się wczytać drzewa. Utworzono nowe.");
            }

        }
    }
}
