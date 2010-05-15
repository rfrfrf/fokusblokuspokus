﻿using System;
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
using Blokus.ViewModel;
using Blokus.Misc;
using Blokus.Logic;

namespace Blokus.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new GameCoordinator();
            InitializeComponent();
        }

        void Board_Click(object sender, BoardClickEventArgs e)
        {
            (DataContext as GameCoordinator).OnBoardClick(e.PiecePosition);
        }

        void Hand_Click(object sender, HandClickEventArgs e)
        {
            (DataContext as GameCoordinator).OnHandControlClick(sender as HandControl, e.Piece);
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            (DataContext as GameCoordinator).OnMouseWheel(e.Delta / Math.Abs(e.Delta));
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            double squareSize = BoardGrid.Width / Board.BoardSize;

            var pos = e.GetPosition((IInputElement)PieceControl.Parent);

            Canvas.SetLeft(PieceControl, pos.X - squareSize/2);
            Canvas.SetTop(PieceControl, pos.Y - squareSize/2);
        }
    }
}