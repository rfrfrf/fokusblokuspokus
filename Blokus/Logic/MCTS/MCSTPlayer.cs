using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.AlphaBeta;

namespace Blokus.Logic.MCTS
{
    public class Node
    {
        public int VisitCount = 0;
        public int WinCount = 0;
        public Dictionary<int, Node> Children;
        public Node Parent;
        
        public bool IsLeaf { get { return Children == null || Children.Count == 0; } }


        public Node this[int i]
        {
            get
            {
                if (Children!=null && Children.ContainsKey(i))
                {
                    return Children[i];
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public class MCSTPlayer : AIPlayer
    {
        private AlphaBetaPlayer playouter = new AlphaBetaPlayer();
        private static Node root = new Node() { Children = new Dictionary<int, Node>() };
        private Random random = new Random();
        private Node currentNode;
        private Player myColor;

        public int MaxDepth
        {
            get { return playouter.MaxDepth; }
            set { playouter.MaxDepth = value; }
        }

        public int MaxTreeRank
        {
            get { return playouter.MaxTreeRank; }
            set { playouter.MaxTreeRank = value; }
        }

        private Move _LastMove;
        private Heuristics _Heursitics = new MCSTHeuristics();

        public override void OnGameStart(GameState gameState)
        {
            currentNode = root;
        }

        public override Move GetMove(GameState gameState)
        {
            if (gameState.AllMoves.Count > 0) // posun currentNode o ruch przeciwnika
            {
                if (currentNode != null)
                {
                    var parent = currentNode;
                    currentNode = currentNode[gameState.AllMoves[gameState.AllMoves.Count - 1].SerializedMove];
                    if (currentNode == null) //dodaj do drzewa ruch przeciwnika
                    {
                        var lastMove = gameState.AllMoves[gameState.AllMoves.Count-1];
                        var node= new Node()
                        { 
                            Children = new Dictionary<int,Node>(),
                            Parent = parent
                        };
                        parent.Children.Add(lastMove.SerializedMove, node);
                            
                    }
                }
            }
            myColor = gameState.CurrentPlayerColor;
            _LastMove = null;
            Mcst(gameState, currentNode);
            if (currentNode != null && _LastMove != null) // posun currentNode o nas ruch
            {
                currentNode = currentNode[_LastMove.SerializedMove];
            }
            return _LastMove;
        }

        public override void OnGameEnd(GameState gameState)
        {
            int result = GameRules.GetWinner(gameState) == myColor ? 1 : 0;
            var node = root;

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

        private double Mcst(GameState gameState, Node node)
        {
            _Heursitics.SortHand(gameState); //posortuj klocki gracza by najlepsze byly na poczatku
            var moves = GameRules.GetMoves(gameState, MaxTreeRank); //pobierz MaxTreeRank pierwszych dostepnych ruchow

            if (moves.Count == 0)
            {
                return GameRules.GetGameResult(gameState); // pozycja koncowa
            }

            if (node != null && !node.IsLeaf)
            {
                var nodeChildren = from c in node.Children select new Move(c.Key);
                moves.AddRange(nodeChildren);
            }

            _Heursitics.SortMoves(gameState, moves); //posortuj ruchy by najlepsze byly na poczatku

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
            Node child;
            if (node!=null)
            {
                child = node[bestMove.SerializedMove];
            }
            else
            {
                child = null;
            }
            if (child == null && node != null)
            {
                var newNode = new Node() { Parent = node, Children = new Dictionary<int, Node>() };

                node.Children.Add(bestMove.SerializedMove, newNode);                
            }

            _LastMove = bestMove;
            return 0;
        }

        private double EvaluateMove(Move move, GameState gameState, Node node)
        {
            Node child;
            double result;
            if (node != null && (child = node[move.SerializedMove]) != null)
            {
                result = -(node.VisitCount - node.WinCount + 0.003) / (node.VisitCount + 10.0);
            }
            else
            {
                gameState.AddMove(move);
                gameState.SwapCurrentPlayer();
                result = -_Heursitics.GetBoardEvaluation(gameState);
                gameState.SwapCurrentPlayer();
                gameState.DelMove(move);
            }
            result += 1;
            return random.NextDouble() * result;
        }

        public override string ToString()
        {
            return "MCTS2";
        }
    }
}
