using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectLib
{
    public class GameField
    {
        public const int CellCount = 625;

        public List<Cell> cells { get; set; } = [];
        public Member? CrossPlayer { get; set; } = null;
        public Member? ZeroPlayer { get; set; } = null;
        public Member? CurrentPlayer { get; set; } = null;
        public GameStatus Status { get; set; } = GameStatus.NotStarted;


        public GameField() 
        {
            for (int i = 0; i < CellCount; i++)
            {
                cells.Add(new Cell(""));
            }
        }
    }
}
