using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sudoku1;

namespace SudokuGUI
{
    public class CellButton : Button
    {
        public int x;
        public int y;
        public Cell? cell;
        public bool highlight = false;
        public CellButton() { }

        public void Update()
        {
            if(cell.Status == EnumCellStatus.WRONG_GUESS)
            {
                Background = Brushes.PaleVioletRed;
            }            
            else if (cell.Status == EnumCellStatus.CORRECT_GUESS)
            {
                Background = Brushes.Transparent;
            }
            else if (cell.Status == EnumCellStatus.TO_GUESS)
            {
                Background = Brushes.LightSteelBlue;
            }
            Content = cell.Num == 0 ? "" : cell.Num;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        Sudoku? sudoku;
        Button? PreviousButton;
        List<CellButton> cellButtons = new List<CellButton>();
        public bool completed;
        public MainWindow()
        {
            InitializeComponent();

            NewGame();
            
        }
        private void NewGame()
        {
            /*            new int[9, 9]{
                            { 3, 0, 6, 5, 0, 8, 4, 0, 0},
                            { 5, 2, 0, 0, 0, 0, 0, 0, 0},
                            { 0, 8, 7, 0, 0, 0, 0, 3, 1},
                            { 0, 0, 3, 0, 1, 0, 0, 8, 0},
                            { 9, 0, 0, 8, 6, 3, 0, 0, 5},
                            { 0, 5, 0, 0, 9, 0, 6, 0, 0},
                            { 1, 3, 0, 0, 0, 0, 2, 5, 0},
                            { 0, 0, 0, 0, 0, 0, 0, 7, 4},
                            { 0, 0, 5, 2, 0, 6, 3, 0, 0}};

                           {{ 8, 0, 0, 0, 0, 0, 0, 0, 0 },
                            { 0, 0, 3, 6, 0, 0, 0, 0, 0 },
                            { 0, 7, 0, 0, 9, 0, 2, 0, 0 },
                            { 0, 5, 0, 0, 0, 7, 0, 0, 0 },
                            { 0, 0, 0, 0, 4, 5, 7, 0, 0 },
                            { 0, 0, 0, 1, 0, 0, 0, 3, 0 },
                            { 0, 0, 1, 0, 0, 0, 0, 6, 8 },
                            { 0, 0, 8, 5, 0, 0, 0, 1, 0 },
                            { 0, 9, 0, 0, 0, 0, 4, 0, 0 }
                            };  */
            sudoku = new EasySudoku(true);
            CreateCellButtons();
        }
        private void CreateCellButtons()
        {
            cellButtons.ForEach(c => Board.Children.Remove(c));
            cellButtons.Clear();
            completed = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    CellButton btn = new CellButton();
                    cellButtons.Add(btn);
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    btn.FontWeight = FontWeights.DemiBold;
                    btn.Foreground = Brushes.Black;
                    btn.Width = 50;
                    btn.Height = 50;
                    btn.Click += CBtn_Click;
                    btn.x = i;
                    btn.y = j;
                    btn.cell = sudoku.Cells.First(c => c.x == i & c.y == j);
                    if (btn.cell.Status == EnumCellStatus.GIVEN)
                    {
                        btn.Background = Brushes.LightGray;
                        btn.IsEnabled = false;
                    }
                    else
                    {
                        btn.Background = Brushes.LightSteelBlue;
                    }
                    btn.Update();
                    Grid.SetRow(btn, i + (int)Math.Floor((decimal)i / 3));
                    Grid.SetColumn(btn, j + (int)Math.Floor((decimal)j / 3));
                    Board.Children.Add(btn);
                }
            }
        }
        private void CBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!completed)
            {
                int num;
                CellButton cbtn = ((CellButton)sender);
                if (sudoku.CurrentNum == 0) 
                {
                    num = cbtn.cell.Num; 
                    sudoku.SetValue(cbtn.cell);
                    ((CellButton)sender).Update();
                    cellButtons.Where(c => c.cell.Num == num).ToList().ForEach(c => c.Update());
                }
                else
                {
                    sudoku.SetValue(cbtn.cell);
                    ((CellButton)sender).Update();
                    num = cbtn.cell.Num;
                    cellButtons.Where(c => c.cell.Num == num).ToList().ForEach(c => c.Update());
                }
            }
            if (sudoku.IsCompleted())
            {
                completed = true;
                string msg = "Wygrałeś!!! Chesz rozpocząć nową grę?";
                MessageBoxResult result =
                      MessageBox.Show(
                        msg,
                        "Wygrałeś",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    NewGame();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int num;
            ChangeHighlight(((Button)sender));

            if(Int32.TryParse((string)((Button)sender).Content, out num))
            {
                sudoku.CurrentNum = num;
            } 
            else
            {
                sudoku.CurrentNum = 0;
            }            
        }

        private void ChangeHighlight(Button b)
        {
            if (PreviousButton != null)
            {
                PreviousButton.Background = Brushes.LightGray;
            }
            PreviousButton = b;
            b.Background = Brushes.PowderBlue; ;
        }
        
        private void New_Game_Click(object sender, RoutedEventArgs e)
        {

            NewGame();
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            Solver.Solve(sudoku.Cells);
            cellButtons.ForEach(c => c.Update());
            completed = true;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var newsudoku = Sudoku.LoadEasy("Board.xml");

                sudoku = newsudoku;
                CreateCellButtons();

            }
            catch
            {
                string msg = "Brak zapisanej gry";
                MessageBoxResult result =
                      MessageBox.Show(
                        msg,
                        "Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!completed)
            {
                string msg = "Chcesz zapisać grę?";
                MessageBoxResult result =
                      MessageBox.Show(
                        msg,
                        "Sudoku",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    sudoku.Save();
                } 
                else if(result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
