using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sudoku1
{

    public class ZlaNazwaPlikuException : Exception
    {
        public ZlaNazwaPlikuException()
        {
        }
    }
    [XmlInclude(typeof(EasySudoku))]
    [Serializable]
    public class Sudoku: ISavable
    {
        protected List<Cell> cells;
        private int currentNum = 0;

        public int CurrentNum { 
            get => currentNum;
            set {
                if ((value <= 9) & (value >= 1))
                {
                    currentNum = value;
                }
                else
                {
                    currentNum = 0;
                }
            }
        }

        public List<Cell> Cells { get => cells; set => cells = value; }

        public Sudoku()
        {
            cells = new List<Cell>();
        }

        protected virtual List<Cell> Generate(int[,] board)
        {
            List<Cell> res = new List<Cell>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    res.Add(new Cell(i, j, board[i, j]));
                }
            }
            return res;
        }

        protected virtual List<Cell> Generate()
        {
            List<Cell> res = new List<Cell>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    res.Add(new Cell(i, j, 0));
                }
            }

            FillValues(res);

            return res;
        }

        private void FillValues(List<Cell> temp)
        {
            FillDiagonal(temp);
            FillRemaining(temp);
        }

        private void FillDiagonal(List<Cell> temp)
        {
            for (int i = 0; i < 9; i += 4)
            {
                var temp1 = temp.FindAll(c => c.group == i);
                FIllBox(temp1);
            }
        }

        private void FIllBox(List<Cell> temp)
        {
            Random rnd = new Random();
            int i1;
            int i2;
            List<int> numbers = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            while (numbers.Count > 0)
            {
                i1 = rnd.Next(0, numbers.Count);
                i2 = rnd.Next(0, numbers.Count);
                temp[i1].Num = numbers[i2];
                temp.RemoveAt(i1);
                numbers.RemoveAt(i2);
            }
        }

        private bool FillRemaining(List<Cell> temp)
        {
            Cell? current = FindUnassigned(temp);
            if (current == null)
            {
                return true;
            }

            for (int i = 1; i <= 9; i++)
            {
                if (IsSafe(temp, current, i))
                {
                    current.Num = i;

                    if (FillRemaining(temp))
                    {
                        return true;
                    }
                    current.Num = 0;
                }
            }

            return false;
        }

        private Cell? FindUnassigned(List<Cell> temp)
        {
            return temp.FirstOrDefault(c => c.Num == 0);
        }

        private bool IsSafe(List<Cell> temp, Cell cell, int num)
        {
            return !temp.Where(c => (c.group == cell.group) | (c.y == cell.y) | (c.x == cell.x)).Any(c => (c.Num == num));
        }

        protected void RemoveNDigits(List<Cell> temp, int n)
        {
            Random rnd = new Random();
            int i1;
            for (int i = 0; i < n; i++)
            {
                i1 = rnd.Next(0, 81);
                if(temp[i1].Num != 0)
                {
                    temp[i1].Num = 0;
                }
                else
                {
                    i--;
                }

            }
        }

        public void SetValue(Cell cell, int num)
        {
            if (cell.Status != EnumCellStatus.GIVEN)
            {
                cell.Num = num;
                if (num == 0)
                {
                    cell.Status = EnumCellStatus.TO_GUESS;
                    Validate();
                }
                else
                {
                    if (IsCellCorrect(cell))
                    {
                        cell.Status = EnumCellStatus.CORRECT_GUESS;
                    }
                    else
                    {
                        Validate();
                    }
                }
            }
        }
        public void SetValue(Cell cell)
        {
            SetValue(cell, currentNum);
        }

        public void SetValue(int x, int y, int num)
        {
            Cell cell = cells.First(c => ((c.x == x) & (c.y == y)));
            SetValue(cell, num);
        }

        public void SetValue(int x, int y)
        {
            SetValue(x, y, currentNum);
        }

        public bool IsCompleted() => !cells.Any(c => c.Status == EnumCellStatus.TO_GUESS | c.Status == EnumCellStatus.WRONG_GUESS);

        public bool IsCellCorrect(Cell cell)
        {
            return !cells.Where(c => ((c.x == cell.x) | (c.y == cell.y) | (c.group == cell.group)) & (c != cell)).Any(c => c.Num == cell.Num);
        }

        public bool IsCellCorrect(int x, int y)
        {
            Cell c = cells.First(c => ((c.x == x) & (c.y == y)));
            return IsCellCorrect(c);
        }

        public void Validate()
        {
            List<Cell> temp = cells.Where(c => (c.Status != EnumCellStatus.GIVEN) & (c.Num != 0)).ToList();
            temp.ForEach(c => c.Status = IsCellCorrect(c) ? EnumCellStatus.CORRECT_GUESS : EnumCellStatus.WRONG_GUESS);

        }

        public void Save()
        {
            Save("Board.xml");
        }
        public void Save(string nazwa)
        {
            using StreamWriter sw = new(nazwa);
            XmlSerializer xs = new(typeof(EasySudoku));
            xs.Serialize(sw, this);
        }
        public static Sudoku? LoadEasy(string nazwa)
        {
            if (!File.Exists(nazwa))
            {
                throw new ZlaNazwaPlikuException();
            }

            using StreamReader sw = new(nazwa);
            XmlSerializer xs = new(typeof(EasySudoku));
            Sudoku es = (Sudoku)xs.Deserialize(sw);

            return es;
        }
        public override string ToString()
        {
            string res = string.Empty;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    res += cells.First(c => (c.x == i & c.y == j)).ToString();
                }
                res += "\n";
            }
            
            return res;
        }
    }

    [XmlInclude(typeof(Sudoku))]
    [Serializable]
    public class EasySudoku : Sudoku
    {
        public EasySudoku() : base() 
        {
        }
        public EasySudoku(bool n) : base() 
        {
            cells = Generate();
        }
        public EasySudoku( int[,] board ) : base()
        {
            cells = Generate(board);
        }

        protected override List<Cell> Generate()
        {
            List<Cell> temp = base.Generate();
            RemoveNDigits(temp, 50);
            temp.FindAll(c => c.Num != 0).ForEach(c => c.Status = EnumCellStatus.GIVEN);
            return temp;
        }   
    }

    public class HardSudoku : Sudoku
    {
        public HardSudoku() : base()
        {
            cells = Generate();
        }

        protected override List<Cell> Generate()
        {
            return base.Generate();
        }
    }
}
