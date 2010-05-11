using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.AlphaBeta;

namespace Blokus.Logic.Scout
{
    class ScoutPlayer : AIPlayer
    {
        private const int MaxDepth = 6;
        private const int MaxTreeRank = 15;
        private Heuristics _Heursitics = new ScoutHeuristics();
        private Move _LastMove;

        public override Move GetMove(GameState gameState)
        {
            _LastMove = null;
            NegaScout(gameState, double.NegativeInfinity, double.PositiveInfinity, 0);
            return _LastMove;
        }

        // condition <= ozn r
        // condition < ozn m
        /*
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
        */

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


        /*
         * function negascout(node, depth, α, β)
    if node is a terminal node or depth = 0
        return the heuristic value of node
    b := β                                          (* initial window is (-β, -α) *)
    foreach child of node
        a := -negascout (child, depth-1, -b, -α)
        if a>α
            α := a
        if α≥β
            return α                                (* Beta cut-off *)
        if α≥b                                      (* check if null-window failed high*)
           α := -negascout(child, depth-1, -β, -α)  (* full re-search *)
           if α≥β
               return α                             (* Beta cut-off *)    
        b := α+1                                    (* set new null window *)             
    return α

         * */
        private double NegaScout (GameState gameState, double alpha, double beta, int d )
        {                     /* compute minimax value of position p */
           double a, b, t;

           if ( d == MaxDepth )
              return _Heursitics.GetBoardEvaluation(gameState);

           _Heursitics.SortHand(gameState); //posortuj klocki gracza by najlepsze byly na poczatku
           var moves = GameRules.GetMoves(gameState, MaxTreeRank); //pobierz MaxTreeRank pierwszych dostepnych ruchow

           if (moves.Count == 0)
           {
               return GameRules.GetGameResult(gameState); // pozycja koncowa
           }

           _Heursitics.SortMoves(gameState, moves); //posortuj ruchy by najlepsze byly na poczatku

           a = alpha;
           b = beta;
           bool first = true;
           double maxVal = double.NegativeInfinity;
           Move bestMove = null;
           foreach(var move in moves)
           {
               //wczuj sie w przeciwnika
              gameState.AddMove(move); //poloz na planszy klocek
              gameState.SwapCurrentPlayer(); //zmien aktywnego gracza na przeciwnego
               //znajdz ruch przeciwnka
              t = -NegaScout(gameState, -b, -a, d + 1);

              if (t > maxVal)
              {
                  maxVal = t;
                  bestMove = move;
              }
              

              if ((t > a) && (t < beta) && !first && (d < MaxDepth - 1))
              {
                  a = -NegaScout(gameState, -beta, -t, d + 1);     /* re-search */
                  if (a > maxVal)
                  {
                      maxVal = a;
                      bestMove = move;
                  }
              }
              first = false;
              //przywroc najlepszy ruch
              gameState.SwapCurrentPlayer(); //przywroc aktywnego gracza                 
              gameState.DelMove(move); //zdejmij klocek z planszy

              

              a = Math.Max( a, t );
              if (a >= beta)
              {
                  _LastMove = bestMove;
                  return a;
              }
              b = a + 1;                      /* set new null window */
           }
           _LastMove = bestMove;
           return a;
        }

        public override string ToString()
        {
            return "Nigga - Scout";
        }
    }
}
