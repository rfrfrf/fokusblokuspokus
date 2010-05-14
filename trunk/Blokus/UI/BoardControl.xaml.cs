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
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        #region DependencyProperty

        public event EventHandler<BoardClickEventArgs> Click;

        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Board.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register("Board", typeof(Board), typeof(BoardControl), new UIPropertyMetadata(null, BoardChanged));

        private static void BoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var boardControl = d as BoardControl;
            if (boardControl == null)
            {
                return;
            }
            boardControl.UpdateBoard();
        }
        #endregion //DependencyProperty

        public BoardControl()
        {
            InitializeComponent();
            CreateBoard();
            UpdateBoard();
        }

        private void CreateBoard()
        {
            int size = Board.BoardSize;

            for (int i = 0; i < size; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            SquareControl square;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    square = new SquareControl(new PiecePosition(i, j), this);
                    Grid.SetColumn(square, i);
                    Grid.SetRow(square, j);
                    MainGrid.Children.Add(square);

                    square.Click += new EventHandler<BoardClickEventArgs>(square_Click);
                }
            }

            var orangeStart = new Ellipse();
            orangeStart.Margin = new Thickness(2);
            orangeStart.Fill = Brushes.Orange;
            Grid.SetColumn(orangeStart, GameRules.OrangeStartPositionX);
            Grid.SetRow(orangeStart, GameRules.OrangeStartPositionY);
            MainGrid.Children.Add(orangeStart);

            var violetStart = new Ellipse();
            violetStart.Margin = new Thickness(2);
            violetStart.Fill = Brushes.Violet;
            Grid.SetColumn(violetStart, GameRules.VioletStartPositionX);
            Grid.SetRow(violetStart, GameRules.VioletStartPositionY);
            MainGrid.Children.Add(violetStart);
        }

        void square_Click(object sender, BoardClickEventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }

        private void UpdateBoard()
        {
            
        }
    }
}
