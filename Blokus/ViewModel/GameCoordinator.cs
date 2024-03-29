﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic;
using Blokus.Misc;
using System.ComponentModel;
using System.Threading;
using Blokus.UI;
using System.Windows;
using Blokus.Logic.MCTS;
using Blokus.Logic.Scout;
using Blokus.Logic.Heuristics;

namespace Blokus.ViewModel
{
    [Serializable]
    public class GameCoordinator : INotifyPropertyChanged
    {
        private PlayerBase _OrangePlayer;
        private PlayerBase _VioletPlayer;
        private GameState _GameState;
        private Move _PreviousMove;
        private BackgroundWorker _Worker;
        private int _CurrentVariantNumber;
        private Piece _CurrentPiece;
        private int _playedGames = 0;
        private double _orangeWins = 0.0;
        private bool _playersSwaped = false;
        private bool _trainingInProgress = false;
        private bool _gameInProgress = false;
        #region Properties

        public bool SwapPlayers {get; set; }

        public double OrangePercentageWins
        {
            get { return PlayedGames==0? 0: OrangeWins / ((double)PlayedGames); }
        }

        public double OrangeWins
        {
            get { return _orangeWins; }
            set
            {
                _orangeWins = value;
                NotifyPropertyChanged("OrangeWins");
            }
        }

        public int PlayedGames
        {
            get { return _playedGames; }
            set 
            { 
                _playedGames = value;
                NotifyPropertyChanged("PlayedGames");
            }
        }

        public PieceVariant CurrentPieceVariant
        {
            get { return CurrentPiece.Variants[_CurrentVariantNumber % CurrentPiece.Variants.Length]; }
        }

        private bool _IsOrangeWinner;

        public bool IsOrangeWinner
        {
            get { return _IsOrangeWinner; }
            set { _IsOrangeWinner = value; NotifyPropertyChanged("IsOrangeWinner"); }
        }

        private bool _IsVioletWinner;

        public bool IsVioletWinner
        {
            get { return _IsVioletWinner; }
            set { _IsVioletWinner = value; NotifyPropertyChanged("IsVioletWinner"); }
        }

        public Piece CurrentPiece
        {
            get { return _CurrentPiece; }
            set 
            { 
                _CurrentPiece = value; 
                NotifyPropertyChanged("CurrentPiece");
                NotifyPropertyChanged("CurrentPieceVariant"); 
            }
        }

        public Player CurrentPlayerColor
        {
            get { return GameState.CurrentPlayerColor; }
        }

        public PlayerBase CurrentPlayer
        {
            set
            {
                switch (GameState.CurrentPlayerColor)
                {
                    case Player.Orange: OrangePlayer = value; break;
                    case Player.Violet: VioletPlayer = value; break;
                }
            }
            get
            {
                switch (GameState.CurrentPlayerColor)
                {
                    case Player.Orange: return OrangePlayer;
                    case Player.Violet: return VioletPlayer;
                }
                return null;
            }
        }

        public HeuristicsBase OrangeHeuristics
        {
            get { return _OrangePlayer.Heuristics; }
            set { _OrangePlayer.Heuristics = value; NotifyPropertyChanged("OrangeHeuristics"); }
        }

        public HeuristicsBase VioletHeuristics
        {
            get { return _VioletPlayer.Heuristics; }
            set { _VioletPlayer.Heuristics = value; NotifyPropertyChanged("VioletHeuristics"); }
        }

        public PlayerBase OrangePlayer
        {
            get { return _OrangePlayer; }
            set 
            {
                if (_OrangePlayer != null)
                {
                    value.Heuristics = _OrangePlayer.Heuristics;
                }
                _OrangePlayer = value; 
                NotifyPropertyChanged("OrangePlayer"); 
            }
        }

        public PlayerBase VioletPlayer
        {
            get { return _VioletPlayer; }
            set 
            {
                if (_VioletPlayer != null)
                {
                    value.Heuristics = _VioletPlayer.Heuristics;
                }
                _VioletPlayer = value;
                NotifyPropertyChanged("VioletPlayer"); 
            }
        }

        public Hand OrangeHand { get; set; }

        public Hand VioletHand { get; set; }

        public GameState GameState
        {
            get { return _GameState; }
            set { _GameState = value; NotifyPropertyChanged("GameState"); }
        }

        public Board Board { get; set; }

        public bool IsGameInProgress
        {
            get { return _Worker != null && _Worker.IsBusy; }
        }

        #endregion // Properties

        public RelayCommand NewGameCommand { get; private set; }

        public RelayCommand TrainCommand { get; private set; }  

        public GameCoordinator()
        {
            NewGameCommand = new RelayCommand((arg) => NewGame(),
                delegate(object arg) { return OrangePlayer != null && VioletPlayer != null && !_trainingInProgress; });

            TrainCommand = new RelayCommand((arg) => NewTraining(),
                delegate(object arg) { return OrangePlayer != null && VioletPlayer != null && !_gameInProgress; });   
        }

        public void OnHandControlClick(HandControl handControl, Piece piece)
        {
            if (handControl.HandOwner != GameState.CurrentPlayerColor)
            {
                return;
            }
            CurrentPiece = piece;
        }

        public void OnMouseWheel(int increment)
        {
            const int MaxVariants = 8;
            while (increment < 0)
            {
                increment += MaxVariants;
            }
            _CurrentVariantNumber = (_CurrentVariantNumber + increment) % MaxVariants;
            NotifyPropertyChanged("CurrentPieceVariant");
        }

        public void OnBoardClick(PiecePosition position)
        {
            if (GameState == null || CurrentPiece == null)
            {
                return;
            }
            
            var player = CurrentPlayer as HumanPlayer;
            if (player == null)
            {
                return;
            }
            var move = new Move(CurrentPiece, position, _CurrentVariantNumber % CurrentPiece.Variants.Length);
            if (player.InvalidateMove(GameState, move))
            {
                player.MoveSemaphore.Release(1);
                CurrentPiece = null;
            }
        }

        private void NewGame() 
        {
            if (_Worker == null || !_Worker.IsBusy)
            {
                IsVioletWinner = false; 
                IsOrangeWinner = false;
                GameState = new GameState() { OrangePlayer = this.OrangePlayer, VioletPlayer = this.VioletPlayer };
                RefreshUI();
                OrangePlayer.OnGameStart(GameState);
                VioletPlayer.OnGameStart(GameState);
                StartGameWorker();
            }
            else
            {
                CurrentPlayer.CancelMove(GameState);
                _Worker.CancelAsync();
            }
        }

        private void NewTraining()
        {
            if (_Worker == null || !_Worker.IsBusy)
            {
                IsVioletWinner = false;
                IsOrangeWinner = false;
                GameState = new GameState() { OrangePlayer = this.OrangePlayer, VioletPlayer = this.VioletPlayer };
                RefreshUI();
                OrangePlayer.OnGameStart(GameState);
                VioletPlayer.OnGameStart(GameState);
                StartGameTrainer();
            }
            else
            {
                CurrentPlayer.CancelMove(GameState);
                _Worker.CancelAsync();
            }
        }

        /// <returns>Zwraca czy gra się zakonczyla</returns>
        private bool MakeMove(bool updateLayout) 
        {
            var player = GameState.CurrentPlayerColor == Player.Orange ? GameState.OrangePlayer : GameState.VioletPlayer;
            Move move = player.GetMove(GameState);
            if (move != null)
            {
                GameState.AddMove(move);
            }
            else if (_PreviousMove == null)
            {
                return true;
            }
            GameState.SwapCurrentPlayer();
            
            _PreviousMove = move;
            if (updateLayout)
            {
                RefreshUI();
            }
            return false;
        }

        private void ShowGameResult()
        {
            var result = GameRules.GetWinner(GameState);
            switch (result)
            {
                case Player.Violet: IsVioletWinner = true; IsOrangeWinner = false; break;
                case Player.Orange: IsVioletWinner = false; IsOrangeWinner = true; break;
                case Player.None: IsVioletWinner = true; IsOrangeWinner = true; break;
            }
        }

        private void RefreshUI()
        {
            GameState = GameState; //odswierzenie planszy

            Board = GameState.Board.Clone();
            OrangeHand = GameState.OrangeHand.Clone();
            VioletHand = GameState.VioletHand.Clone();
            NotifyPropertyChanged("Board");
            NotifyPropertyChanged("OrangeHand");
            NotifyPropertyChanged("VioletHand");
            NotifyPropertyChanged("CurrentPlayerColor");
        }

        private void StartGameWorker()
        {
            _Worker = new BackgroundWorker();
            _Worker.WorkerSupportsCancellation = true;
            _Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GameWorkerCompleted);
            _Worker.DoWork += new DoWorkEventHandler(GameWorker);
            _Worker.RunWorkerAsync(null);
            NotifyPropertyChanged("IsGameInProgress");
        }

        private void StartGameTrainer()
        {
            PlayedGames = 0;
            OrangeWins = 0;
            _Worker = new BackgroundWorker();
            _Worker.WorkerSupportsCancellation = true;
            _Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GameWorkerCompleted);
            _Worker.DoWork += new DoWorkEventHandler(GameTrainer);
            _Worker.RunWorkerAsync(null);            
            NotifyPropertyChanged("IsGameInProgress");
        }

        private void GameWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NotifyPropertyChanged("IsGameInProgress");
            OrangePlayer.OnGameEnd(GameState);
            VioletPlayer.OnGameEnd(GameState);
            ShowGameResult();
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
            }
        }

        private void GameTrainer(object sender, DoWorkEventArgs e)
        {
            var orange = OrangePlayer;
            var violet = VioletPlayer;
            _playersSwaped = false;

            _trainingInProgress = true;
            while (!_Worker.CancellationPending)
            {
                if (MakeMove(false))
                {
                    var winner = GameRules.GetWinner(GameState);

                    if (winner == Player.None)
                    {
                        OrangeWins += 0.5;
                    }
                    else if (winner == Player.Orange ^ _playersSwaped)
                    {
                        OrangeWins++;
                    }

                    IsVioletWinner = false;
                    IsOrangeWinner = false;
                    orange.OnGameEnd(GameState);
                    violet.OnGameEnd(GameState);

                    if (SwapPlayers)
                    {
                        var tmp = orange;
                        orange = violet;
                        violet = tmp;
                        _playersSwaped = !_playersSwaped;
                    }

                    GameState = new GameState() { OrangePlayer = orange, VioletPlayer = violet };
                    orange.OnGameStart(GameState);
                    violet.OnGameStart(GameState);
                    PlayedGames++;
                    NotifyPropertyChanged("OrangePercentageWins");
                }
            }
            _trainingInProgress = false;
            NotifyPropertyChanged("OrangePercentageWins");
        }

        private void GameWorker(object sender, DoWorkEventArgs e)
        {
            _gameInProgress = true;
            while (!_Worker.CancellationPending)
            {
                if (MakeMove(true))
                {
                    break;
                }
            }
            _gameInProgress = false;
            NotifyPropertyChanged("OrangePercentageWins");
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
