using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Blokus.Logic
{
    public class HumanPlayer : PlayerBase
    {
        public Semaphore MoveSemaphore { get; set; }
        private List<Move> _Moves;
        private Move _LastMove;

        #region PlayerBase Members

        public override Move GetMove(GameState gameState)
        {
            _Moves = GameRules.GetMoves(gameState);
            if (_Moves.Count == 0)
            {
                return null;
            }
            MoveSemaphore.WaitOne();

            return _LastMove;
        }

        public override void CancelMove()
        {
            _LastMove = null;
            MoveSemaphore.Release();
        }

        public override void OnGameStart() 
        {
            MoveSemaphore = new Semaphore(0, 1);
        }

        public override void OnGameEnd() { }

        #endregion

        public bool InvalidateMove(GameState gameState, Move move)
        {
            if (_Moves.Contains(move))
            {
                _LastMove = move;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Human";
        }

    }
}
