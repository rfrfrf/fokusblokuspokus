using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.AlphaBeta
{
    class AlphaBetaPlayer : AIPlayer
    {
        /*
         * function alphabeta(node, depth, α, β)         
                (* β represents previous player best choice - doesn't want it if α would worsen it *)
                if  depth = 0 "or" node is a terminal node
                    return the heuristic value of node
                foreach child of node
                    α := max(α, -alphabeta(child, depth-1, -β, -α))     
                    (* use symmetry, -β becomes subsequently pruned α *)
                    if β≤α
                        break                             (* Beta cut-off *)
                return α

            (* Initial call *)
            alphabeta(origin, depth, -infinity, +infinity)
         */
        private int _MaxDepth = 2;
        private int _MaxTreeRank = 666;

        public int MaxDepth
        {
            get { return _MaxDepth; }
            set { _MaxDepth = value; }
        }        

        public int MaxTreeRank
        {
            get { return _MaxTreeRank; }
            set { _MaxTreeRank = value; }
        }


        private Move _LastMove;
        private AlphaBetaHeuristics _Heursitics = new AlphaBetaHeuristics();

        public override Move GetMove(GameState gameState)
        {
            _LastMove = null;
            AlphaBeta(gameState, double.NegativeInfinity, double.PositiveInfinity, MaxDepth);
            return _LastMove;
        }        

        private double AlphaBeta(GameState gameState, double alpha, double beta, int depth )
        {
            if (depth == 0) //osiagnieto maksymalny poziom rekurencji
            {
                return _Heursitics.GetBoardEvaluation(gameState);
            }
            _Heursitics.SortHand(gameState); //posortuj klocki gracza by najlepsze byly na poczatku
            var moves = GameRules.GetMoves(gameState, MaxTreeRank); //pobierz MaxTreeRank pierwszych dostepnych ruchow

            if (moves.Count == 0)
            {
                return GameRules.GetGameResult(gameState); // pozycja koncowa
            }

            _Heursitics.SortMoves(gameState, moves); //posortuj ruchy by najlepsze byly na poczatku

            foreach (var move in moves)
            {               
                //wczuj sie w przeciwnika
                gameState.AddMove(move); //poloz na planszy klocek
                gameState.SwapCurrentPlayer(); //zmien aktywnego gracza na przeciwnego
                var lastMoveCopy = _LastMove; //zapamietaj najlepszy ruch bo rekurencyjne wywolanie go nadpisze
                //znajdz ruch przeciwnka
                double newalpha = -AlphaBeta(gameState, -beta, -alpha, depth - 1);
                //posprzataj
                _LastMove = lastMoveCopy; //przywroc najlepszy ruch
                gameState.SwapCurrentPlayer(); //przywroc aktywnego gracza                 
                gameState.DelMove(move); //zdejmij klocek z planszy

                if (newalpha > alpha)
                {
                    alpha = newalpha;
                    _LastMove = move;
                }
                if (beta < alpha)
                {
                    break;
                }
            }
            return alpha;
        }

        public override string ToString()
        {
            return "Alpha Beta";
        }
    }
}
