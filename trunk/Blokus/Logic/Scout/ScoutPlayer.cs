using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.AlphaBeta;

namespace Blokus.Logic.Scout
{
    class ScoutPlayer : AIPlayer
    {
        private const int MaxTreeRank = 7;
        private ScoutHeuristics _Heursitics = new ScoutHeuristics();
        private Move _LastMove;

        public override Move GetMove(GameState gameState)
        {
            _LastMove = null;
            Scout(gameState);
            return _LastMove;
        }

        // condition <= ozn r
        // condition < ozn m
        private bool TestNode(GameState gameState, double value, char condition)
        {
            _Heursitics.SortHand(gameState);
            var moves = GameRules.GetMoves(gameState, MaxTreeRank);

            if (moves.Count == 0)
                switch (condition)
                {
                    case 'm':
                        return (_Heursitics.GetBoardEvaluation(gameState) > value);
                    case 'r':
                        return (_Heursitics.GetBoardEvaluation(gameState) >= value);
                }

            _Heursitics.SortMoves(gameState, moves);
            if (gameState.CurrentPlayerColor == Player.Orange)
                foreach (var move in moves)
                {
                    gameState.AddMove(move);
                    if (TestNode(gameState, value, condition) == true)
                        return true;
                }

            if (gameState.CurrentPlayerColor == Player.Violet)
                foreach (var move in moves)
                {
                    gameState.AddMove(move);
                    if (TestNode(gameState, value, condition) == false)
                        return false;
                }
            return false;
        }

        private double Scout(GameState gameState)
        {
            double value;
            var moves = GameRules.GetMoves(gameState, MaxTreeRank);
            
            if (moves.Count == 0)
                return _Heursitics.GetBoardEvaluation(gameState);
            
            gameState.AddMove(moves[0]);
            value = Scout(gameState);

            foreach (var move in moves)
            {
                gameState.AddMove(move);
                value = Scout(gameState);

                if ((gameState.CurrentPlayerColor == Player.Orange && TestNode(gameState, value, 'm') == true) || (gameState.CurrentPlayerColor == Player.Violet && TestNode(gameState, value, 'r') == false))
                    value = Scout(gameState);
            }
            return value;
        }

        public override string ToString()
        {
            return "Scout";
        }
    }
}
