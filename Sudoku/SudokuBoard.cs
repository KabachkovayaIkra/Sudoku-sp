using System;
using System.Linq;

namespace Sudoku
{
    public enum Difficulty { Easy, Medium, Hard }

    public class SudokuBoard
    {
        public Cell[,] Cells { get; private set; }
        private Random rnd = new Random();

        public SudokuBoard()
        {
            Cells = new Cell[9, 9];
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    Cells[i, j] = new Cell(i, j);
        }

        public void GenerateFullBoard()
        {
            int[,] matrix = new int[9, 9];
            Fill(matrix);
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    Cells[i, j].RealValue = matrix[i, j];
        }

        private bool Fill(int[,] grid)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] == 0)
                    {
                        var nums = Enumerable.Range(1, 9).OrderBy(x => rnd.Next()).ToList();
                        foreach (var num in nums)
                        {
                            if (IsValid(grid, i, j, num))
                            {
                                grid[i, j] = num;
                                if (Fill(grid)) return true;
                                grid[i, j] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsValid(int[,] grid, int row, int col, int num)
        {
            for (int i = 0; i < 9; i++)
                if (grid[row, i] == num || grid[i, col] == num)
                    return false;

            int boxRow = (row / 3) * 3;
            int boxCol = (col / 3) * 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (grid[boxRow + i, boxCol + j] == num)
                        return false;

            return true;
        }

        public void ApplyDifficulty(Difficulty difficulty)
        {
            int openCount = 32;
            if (difficulty == Difficulty.Easy) openCount = 40;
            else if (difficulty == Difficulty.Medium) openCount = 32;
            else if (difficulty == Difficulty.Hard) openCount = 24;

            // Сначала устанавливаем значения
            foreach (var cell in Cells)
            {
                cell.DisplayedValue = cell.RealValue;
                cell.IsInitial = true;
            }

            // Скрываем случайные ячейки
            int totalToRemove = 81 - openCount;
            while (totalToRemove > 0)
            {
                int row = rnd.Next(0, 9);
                int col = rnd.Next(0, 9);
                var cell = Cells[row, col];
                if (cell.DisplayedValue != null)
                {
                    cell.DisplayedValue = null;
                    cell.IsInitial = false;
                    totalToRemove--;
                }
            }
        }

    }
}
