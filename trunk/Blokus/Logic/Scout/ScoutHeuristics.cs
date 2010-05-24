﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokus.Logic.Scout
{
    class ScoutHeuristics : Heuristics
    {
        private bool nonDeterministic = false;
        private static Random _Random = new Random();

        int[] indices;
        public ScoutHeuristics()
        {
            CreateOrder();
        }

        private void CreateOrder()
        {
            int[] order = new int[] { 19, 15, 16, 12, 18, 21, 11, 17, 20, 13, 14, 10, 8, 5, 6, 3, 9, 7, 4, 2, 1 };
            indices = new int[order.Length];
            for (int i = 0; i < order.Length; i++)
            {
                indices[order[i] - 1] = i;
            }
        }

        public ScoutHeuristics(bool nonDeterministic)
        {
            CreateOrder();
            this.nonDeterministic = nonDeterministic;
        }

        /// <summary>
        /// dla pomaranczowego gracza promuje ruchy w kierunku lewego gornego rogu planszy
        /// dla fioletowego gracza promuje ruchy w kierunku prawego dolnego rogu planszy
        /// </summary>
        public override double GetBoardEvaluation(GameState gameState)
        {
            double add = 0.0;
            if (nonDeterministic)
            {
                add = _Random.NextDouble();
                add *= add;
                add *= add;
            }
            return GameRules.GetMoves(gameState).Count * (1 + add) * 0.01;
        }


        /// <summary>
        /// na poczatku listy klockow ustawia klocki o najwiekszej liczbie rogow lub najwiekszej liczbie elementow
        /// </summary>
        /// <param name="gameState"></param>
        public override void SortHand(GameState gameState)
        {
            gameState.CurrentPlayerHand.HandPieces.Sort((x, y) =>
            {
                return indices[x.Id-1].CompareTo(indices[y.Id-1]);
            });
        }

        /// <summary>
        /// dla pomaranczowego gracza promuje ruchy w kierunku lewego gornego rogu planszy
        /// dla fioletowego gracza promuje ruchy w kierunku prawego dolnego rogu planszy
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="moves"></param>
        public override void SortMoves(GameState gameState, List<Move> moves)
        {
            int phase = 1;
            if (gameState.AllMoves.Count >= 8)
            {
                phase = 2;
            }
            if (gameState.AllMoves.Count >= 12)
            {
                phase = 3;
            }
            if (gameState.AllMoves.Count >= 24)
            {
                phase = 4;
            } 
            switch(phase)
            {
                case 1: Phase1(gameState, moves); break;
                case 2: Phase2(gameState, moves); break;
                case 3: Phase3(gameState, moves); break;
               // case 4: Phase4(gameState, moves); break;
            }

        }

        private static void Phase1(GameState gameState, List<Move> moves)
        {
            int halfsize = Board.BoardSize / 2 - 1;
            moves.Sort((x, y) =>
            {
                int a = Math.Abs(halfsize - x.Position.X) + Math.Abs(halfsize - x.Position.Y);
                int b = Math.Abs(halfsize - y.Position.X) + Math.Abs(halfsize - y.Position.Y);
                if (a != b)
                {
                    return b.CompareTo(a);
                }
                return y.PieceVariant.Squares.Length.CompareTo(x.PieceVariant.Squares.Length);
            });
        }

        private static void Phase2(GameState gameState, List<Move> moves)
        {

            int multiplier = gameState.CurrentPlayerColor == Player.Violet ? 1 : -1;
            moves.Sort((x, y) =>
            {
                int a = x.Position.X + x.Position.Y;
                int b = y.Position.X + y.Position.Y;
                if (a != b)
                {
                    return b.CompareTo(a) * multiplier;
                }
                return y.PieceVariant.Squares.Length.CompareTo(x.PieceVariant.Squares.Length) * multiplier;
            });
        }

        private static void Phase3(GameState gameState, List<Move> moves)
        {
            moves.Sort((x, y) =>
            {
                int a = Math.Abs(Board.BoardSize - 2 - x.Position.X - x.Position.Y);
                int b = Math.Abs(Board.BoardSize - 2 - y.Position.X - y.Position.Y);
                if (a != b)
                {
                    return b.CompareTo(a);
                }
                return y.PieceVariant.Squares.Length.CompareTo(x.PieceVariant.Squares.Length);
            });
        }
    }
}
