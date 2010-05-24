using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic.AlphaBeta;
using Blokus.Logic.Heuristics;

namespace Blokus.Logic.Scout
{
    class ScoutPlayer : AIPlayer
    {
        private HeuristicsBase _Heursitics;
        private Move _LastMove;
        private int _MaxDepth = 2;
        private int _MaxTreeRank = 5;

        public override HeuristicsBase Heuristics
        {
            get { return _Heursitics; }
            set { _Heursitics = value; }
        }

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

        public override Move GetMove(GameState gameState)
        {
            _LastMove = null;
            NegaScout(gameState, double.NegativeInfinity, double.PositiveInfinity, 0);
            return _LastMove;
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

           if ( d >= MaxDepth )
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
            return "Scout";
        }
    }
}
