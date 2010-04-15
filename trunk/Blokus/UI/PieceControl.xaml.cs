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

namespace Blokus.UI
{
    /// <summary>
    /// Interaction logic for PieceControl.xaml
    /// </summary>
    public partial class PieceControl : UserControl
    {     

        private int SquareSize { get; set; }

        public PieceVariant Piece
        {
            get { return (PieceVariant)GetValue(PieceProperty); }
            set { SetValue(PieceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Piece.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PieceProperty =
            DependencyProperty.Register("Piece", typeof(PieceVariant), typeof(PieceControl), new UIPropertyMetadata(null, PieceChanged));



        public Brush PieceColor
        {
            get { return (Brush)GetValue(PieceColorProperty); }
            set { SetValue(PieceColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PieceColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PieceColorProperty =
            DependencyProperty.Register("PieceColor", typeof(Brush), typeof(PieceControl), new UIPropertyMetadata(null));



        private static void PieceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pieceControl = d as PieceControl;
            if (pieceControl == null)
            {
                return;
            }
            pieceControl.UpdatePiece();
        }

        public PieceControl()
        {
            SquareSize = 22;

            InitializeComponent();
        }

        public void UpdatePiece()
        {
            MainCanvas.Children.Clear();

            foreach (var square in Piece.Squares)
            {
                var border = new Border()
                {
                    Background = Brushes.AntiqueWhite,
                    Width = SquareSize,
                    Height = SquareSize
                };
                Canvas.SetLeft(border, SquareSize * square.X);
                Canvas.SetTop(border, SquareSize * square.Y);
                MainCanvas.Children.Add(border);
            }
        }
    }
}
