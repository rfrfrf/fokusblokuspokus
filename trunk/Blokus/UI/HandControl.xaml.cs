using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Blokus.Logic;
using Blokus.Misc;

namespace Blokus.UI
{
    /// <summary>
    /// Interaction logic for HandControl.xaml
    /// </summary>
    public partial class HandControl : UserControl
    {
        public event EventHandler<HandClickEventArgs> Click;

        private const int PieceSizeX = 5;
        private const int PieceSizeY = 6;
        private const int PieceCountX = 3;
        private const int PieceCountY = 7;

        private const int CountX = PieceSizeX * PieceCountX - 1;
        private const int CountY = PieceSizeY * PieceCountY - 1;

        private int _VariantNumber = 0;

        #region DependencyProperty

        public Player HandOwner
        {
            get { return (Player)GetValue(HandOwnerProperty); }
            set { SetValue(HandOwnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HandOwner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HandOwnerProperty =
            DependencyProperty.Register("HandOwner", typeof(Player), typeof(HandControl), new UIPropertyMetadata(null));

        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Board.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register("Board", typeof(Board), typeof(HandControl), new UIPropertyMetadata(null));

        public Hand Hand
        {
            get { return (Hand)GetValue(HandProperty); }
            set { SetValue(HandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Board.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HandProperty =
            DependencyProperty.Register("Hand", typeof(Hand), typeof(HandControl), new UIPropertyMetadata(null, HandChanged));

        private static void HandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var handControl = d as HandControl;
            if (handControl == null)
            {
                return;
            }
            handControl.UpdateBoard();
        }
        #endregion //DependencyProperty

        public HandControl()
        {
            InitializeComponent();
            CreateBoard();
            UpdateBoard();
        }

        private void CreateBoard()
        {
            int squaresize = 8;
            
            Width = squaresize * CountX;
            Height = squaresize * CountY;

            for (int i = 0; i < CountX; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < CountY; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            SquareControl square;

            for (int i = 0; i < CountX; i++)
            {
                for (int j = 0; j < CountY; j++)
                {
                    square = new SquareControl(new PiecePosition(i, j), this);
                    Grid.SetColumn(square, i);
                    Grid.SetRow(square, j);
                    MainGrid.Children.Add(square);

                    square.Click += new EventHandler<BoardClickEventArgs>(square_Click);
                }
            }
        }

        void square_Click(object sender, BoardClickEventArgs e)
        {
            if (Click != null && Hand!=null)
            {
                int variantX = e.PiecePosition.X / PieceSizeX;
                int variantY = e.PiecePosition.Y / PieceSizeY;

                int ind = variantX + variantY * PieceCountX;
                if (ind >= Hand.HandPieces.Count)
                {
                    return;
                }
                Click(this, new HandClickEventArgs(Hand.HandPieces[ind]));
            }
        }

        private void UpdateBoard()
        {
            if (Hand == null)
            {
                return;
            }

            int x = 0;
            int y = 0;

            Board board = new Board(CountX, CountY);

            foreach (var piece in Hand.HandPieces)
            {
                if (_VariantNumber < piece.Variants.Count())
                {
                    board.PlacePiece(new Move(piece, new PiecePosition(x * PieceSizeX, y * PieceSizeY), _VariantNumber)
                        ,HandOwner);
                }
                x++;
                if (x >= PieceCountX)
                {
                    x = 0;
                    y++;
                }
            }

            Board = board;
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    _VariantNumber++;
        //    if (_VariantNumber > 7)
        //    {
        //        _VariantNumber = 0;
        //    }
        //    UpdateBoard();
        //}
    }
}
