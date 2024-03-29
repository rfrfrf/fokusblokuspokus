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
using Blokus.Logic.MCTS;
using Blokus.Logic.MCTS2v2;

namespace Blokus.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filename = "tree.dat";
        string filename2 = "tree2.dat";
        public MainWindow()
        {
            this.DataContext = new GameCoordinator();
            InitializeComponent();
 
            MCSTPlayer.ReadTree(filename2);
            MCTS2v2Player.ReadTree(filename);
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MultipleTree.SaveTree(MonteCarloTreeSearchPlayer.filename);
            if (MessageBox.Show("Czy chcesz zapisać drzewka?", "Uwaga", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MCSTPlayer.SaveTree(filename2);
                MCTS2v2Player.SaveTree(filename);
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).SelectedIndex = 3;
        }

        private void ComboBox_Loaded_1(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).SelectedIndex = 2;
        }

        private void heuristicsComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).SelectedIndex = 1;
        }
    }
}
