using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic
{
    [Serializable]
    public class GameState
    {
        public Player CurrentPlayerColor { get; set; }
        public Hand OrangeHand { get; set; }
        public Hand VioletHand { get; set; }
        public Board Board { get; set; }
        public bool IsLastOrangeMoveMonomino { private set; get; }
        public bool IsLastVioletMoveMonomino { private set; get; }

        public List<Move> AllMoves { private set; get; }

        private bool IsLastMoveMonomino
        {
            set
            {
                switch (CurrentPlayerColor)
                {
                    case Player.Orange: IsLastOrangeMoveMonomino = value; break;
                    case Player.Violet: IsLastVioletMoveMonomino = value; break;
                }
            }
        }

        public Hand CurrentPlayerHand
        {
            get
            {
                switch (CurrentPlayerColor)
                {
                    case Player.Orange: return OrangeHand;
                    case Player.Violet: return VioletHand;
                }
                return null;
            }
        }

        public GameState()
        {
            CurrentPlayerColor = Player.Orange;
            Board = new Board();
            OrangeHand = new Hand();
            VioletHand = new Hand();
            AllMoves = new List<Move>();
        }

        public void AddMove(Move move)
        {
            IsLastMoveMonomino = move.PieceVariant.Squares.Length == 1;
            Board.PlacePiece(move, CurrentPlayerColor);
            CurrentPlayerHand.Remove(move);
            AllMoves.Add(move);
        }

        public void DelMove(Move move)
        {
            Board.RemovePiece(move);
            CurrentPlayerHand.Add(move);
            AllMoves.Remove(move);
        }

        public void SwapCurrentPlayer()
        {
            if (CurrentPlayerColor == Player.Orange)
            {
                CurrentPlayerColor = Player.Violet;
            }
            else if (CurrentPlayerColor == Player.Violet)
            {
                CurrentPlayerColor = Player.Orange;
            }
        }
    }
}
