using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic;
using Blokus.Misc;
using System.ComponentModel;
using System.Threading;
using Blokus.UI;
using System.Windows;

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
        private int _orangeWins = 0;
        
        #region Properties
        
        public int OrangeWins
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

        public PlayerBase OrangePlayer
        {
            get { return _OrangePlayer; }
            set { _OrangePlayer = value; NotifyPropertyChanged("OrangePlayer"); }
        }

        public PlayerBase VioletPlayer
        {
            get { return _VioletPlayer; }
            set { _VioletPlayer = value; NotifyPropertyChanged("VioletPlayer"); }
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
                delegate(object arg) { return OrangePlayer != null && VioletPlayer != null; });

            TrainCommand = new RelayCommand((arg) => NewTraining(),
                delegate(object arg) { return OrangePlayer != null && VioletPlayer != null; });   
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
                GameState = new GameState();
                RefreshUI();
                OrangePlayer.OnGameStart();
                VioletPlayer.OnGameStart();
                StartGameWorker();
            }
            else
            {
                CurrentPlayer.CancelMove();
                _Worker.CancelAsync();
            }
        }

        private void NewTraining()
        {
            if (_Worker == null || !_Worker.IsBusy)
            {
                IsVioletWinner = false;
                IsOrangeWinner = false;
                GameState = new GameState();
                RefreshUI();
                OrangePlayer.OnGameStart();
                VioletPlayer.OnGameStart();
                StartGameTrainer();
            }
            else
            {
                CurrentPlayer.CancelMove();
                _Worker.CancelAsync();
            }
        }

        /// <returns>Zwraca czy gra się zakonczyla</returns>
        private bool MakeMove(bool updateLayout) 
        {
            Move move = CurrentPlayer.GetMove(GameState);
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
            OrangePlayer.OnGameEnd();
            VioletPlayer.OnGameEnd();
            ShowGameResult();
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
            }
        }

        private void GameTrainer(object sender, DoWorkEventArgs e)
        {
            while (!_Worker.CancellationPending)
            {
                if (MakeMove(false))
                {
                    if (GameRules.GetWinner(GameState) == Player.Orange)
                    {
                        OrangeWins++;
                    }
                    IsVioletWinner = false;
                    IsOrangeWinner = false;
                    GameState = new GameState();
                    PlayedGames++;
                    OrangePlayer.OnGameEnd();
                    VioletPlayer.OnGameEnd();
                    OrangePlayer.OnGameStart();
                    VioletPlayer.OnGameStart();
                }
            }
        }

        private void GameWorker(object sender, DoWorkEventArgs e)
        {
            while (!_Worker.CancellationPending)
            {
                if (MakeMove(true))
                {
                    break;
                }
            }
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
