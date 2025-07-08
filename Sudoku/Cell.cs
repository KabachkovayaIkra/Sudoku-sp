using System.Collections.Generic;

namespace Sudoku
{
    public class Cell
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int RealValue { get; set; }
        public int? DisplayedValue { get; set; }
        public bool IsInitial { get; set; }
        public HashSet<int> Notes { get; set; }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
            Notes = new HashSet<int>();
        }
    }
}
