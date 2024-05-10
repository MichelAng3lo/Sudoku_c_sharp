using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku1
{
    public enum EnumCellStatus
    {
        TO_GUESS,
        GIVEN,
        CORRECT_GUESS,
        WRONG_GUESS
    }
    public class Cell
    {
        public int x;
        public int y;
        int _num;
        public int group;
        private EnumCellStatus status;

        public Cell() { }
        public Cell(int x, int y, int num)
        {
            this.x = x;
            this.y = y;
            group = (int)Math.Floor((decimal)y / 3) * 3 + (int)Math.Floor((decimal)x / 3);
            _num = num;
            if (num != 0) { Status = EnumCellStatus.GIVEN; } else { Status = EnumCellStatus.TO_GUESS; }
        }

        public int Num { 
            get => _num; 
            set 
            {
                if (Status != EnumCellStatus.GIVEN)
                {
                    if ((value <= 9) & (value >= 1))
                    {
                        _num = value;
                    }
                    else
                    {
                        Status = EnumCellStatus.TO_GUESS;
                        _num = 0;
                    }
                }
            } 
        }

        public EnumCellStatus Status { get => status; set => status = value; }

        public override string ToString()
        {
            return $"{Num} ";
        }
    }
}
