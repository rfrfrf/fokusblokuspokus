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
    /// Interaction logic for SquareControl.xaml
    /// </summary>
    public partial class SquareControl : UserControl
    {
        private static Brush background = (Brush)new BrushConverter().ConvertFrom("#60693ba7");
        #region DependencyProperty
        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Board.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register("Board", typeof(Board), typeof(SquareControl), new UIPropertyMetadata(null, BoardChanged));

        private static void BoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var squareControl = d as SquareControl;
            var board = e.NewValue as Board;
            if (squareControl == null || board == null)
            {
                return;
            }
            Brush brush;
            switch(board.GetElementAt(squareControl.Position))
            {
                case Player.Orange: brush = Brushes.Orange; break;
                case Player.Violet : brush = Brushes.Violet; break;
             /*   case Player.HighlightedOrange: brush = Brushes.Yellow; break;
                case Player.HighlightedViolet: brush = Brushes.Lavender; break;*/
                default: brush = background; break;
            }
            squareControl.MainButton.Background = brush;
        }
        #endregion //DependencyProperty

        public event EventHandler<BoardClickEventArgs> Click;

        public PiecePosition Position { get; set; }

        public SquareControl(PiecePosition position, UserControl owner)
        {
            Position = position;

            Binding boardBinding = new Binding("Board");
            boardBinding.Source = owner;
            SetBinding(SquareControl.BoardProperty, boardBinding);
            
            InitializeComponent();

            MainButton.Click += new RoutedEventHandler(MainButton_Click);
        }

        void MainButton_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null)
            {
                Click(this, new BoardClickEventArgs(Position));
            }
        }

    }
}
