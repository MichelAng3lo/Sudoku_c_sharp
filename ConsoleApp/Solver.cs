using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku1
{
    public class Solver
    {
        private static int[,] board = new int[9, 9];

        //Dla optymalniejszego działania algorytmu z listy tworzymy tablicę
        static void MakeBoard(List<Cell> cells)
        {
            for(int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Cell c = cells.First(c => (c.x == i & c.y == j));

                    board[i, j] = (c.Status == EnumCellStatus.GIVEN) ? c.Num : 0;
                }
            }
        }

        //Z tablicy tworzymy spowrotem listę
        static List<Cell> MakeList(List<Cell> cells)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Cell c = cells.First(c => (c.x == i & c.y == j));

                    if (c.Status != EnumCellStatus.GIVEN)
                    {
                        c.Num = board[i, j];
                        c.Status = EnumCellStatus.CORRECT_GUESS;
                    }
                }
            }
            return cells;
        }
        public static void Solve(List<Cell> cells)
        {
            MakeBoard(cells);
            if (SolveSudoku(board))
            {
                MakeList(cells);
            }
        }

        static bool IsSafe(int[,] board, int row, int col, int num)
        {
            // Sprawdzamy czy liczba jest już w wierszu lub kolumnie
            for (int x = 0; x < 9; x++)
            {
                if (board[row, x] == num || board[x, col] == num)
                    return false;
            }

            // Sprawdzamy czy liczba jest już w danym podkwadracie 3x3

            int boxStartRow = row - row % 3;
            int boxStartCol = col - col % 3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[boxStartRow + i, boxStartCol + j] == num)
                        return false;
                }
            }

            return true;
        }

        static bool SolveSudoku(int[,] board)
        {
            int row, col;

            // Sprawdzamy czy są jeszcze puste pola
            if (!FindUnassignedLocation(board, out row, out col))
                return true; // Sudoku jest już rozwiązane

            // Przypisujemy wartości od 1 do 9 do pustego pola
            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(board, row, col, num))
                {
                    // Jeśli w polu może zostac wpisana wartość num, przypisujemy ją i rekurencyjnie rozwiązujemy resztę Sudoku
                    board[row, col] = num;

                    if (SolveSudoku(board))
                        return true;

                    // Jeśli przypisanie wartości nie prowadzi do rozwiązania, cofamy zmianę i idziemy do kolejnej wartości
                    board[row, col] = 0;
                }
            }

            // Jeśli żadna liczba nie pasuje, cofamy się do poprzedniego pola
            return false;
        }

        static bool FindUnassignedLocation(int[,] board, out int row, out int col)
        {
            for (row = 0; row < 9; row++)
            {
                for (col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                        return true;
                }
            }

            row = col = 0;
            return false;
        }
    }
}
