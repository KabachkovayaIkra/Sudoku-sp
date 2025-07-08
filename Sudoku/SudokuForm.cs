using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Sudoku: Form
    {
        private SudokuBoard board;
        private UserControlCell[,] cells;
        private Timer gameTimer;
        private TimeSpan timeElapsed = TimeSpan.Zero;
        private int checkAttempts = 0;
        public Sudoku()
        {
            InitializeComponent();
        }
        private void InitializeBoard(Difficulty difficulty)
        {
            board = new SudokuBoard();
            board.GenerateFullBoard();
            board.ApplyDifficulty(difficulty);

            cells = new UserControlCell[9, 9];
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 9;
            tableLayoutPanel1.ColumnCount = 9;
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.Size = new Size(415, 415);
            tableLayoutPanel1.Location = new Point(7, 35);
            tableLayoutPanel1.Margin = new Padding(3, 3, 3, 3);

            for (int i = 0; i < 9; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 45));
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var cell = new UserControlCell(board.Cells[i, j]);
                    cells[i, j] = cell;
                    cell.Width = 45;
                    cell.Height = 45;
                    cell.TabStop = true;
                    cell.TabIndex = i * 9 + j;
                    int rowCopy = i;
                    int colCopy = j;
                    cell.GotFocus += (s, e) => HighlightGroup(rowCopy, colCopy);
                    cell.LostFocus += (s, e) => ClearHighlights();


                    var panel = new Panel();
                    panel.Margin = new Padding(0);
                    panel.Padding = new Padding(
                        (j % 3 == 0) ? 2 : 0,
                        (i % 3 == 0) ? 2 : 0,
                        (j == 8) ? 2 : 0,
                        (i == 8) ? 2 : 0);
                    panel.Width = 45;
                    panel.Height = 45;
                    panel.BackColor = Color.Black;

                    cell.Dock = DockStyle.Fill;
                    panel.Controls.Add(cell);

                    tableLayoutPanel1.Controls.Add(panel, j, i);
                }
            }


            StartTimer();
        }
        private void HighlightGroup(int row, int col)
        {
            if (cells == null || row < 0 || row >= 9 || col < 0 || col >= 9)
            {
                MessageBox.Show($"HighlightGroup error: row={row}, col={col}", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    cells[row, i].BackColor = Color.LightYellow;
                    cells[i, col].BackColor = Color.LightYellow;
                }

                int startRow = (row / 3) * 3;
                int startCol = (col / 3) * 3;
                for (int i = startRow; i < startRow + 3; i++)
                    for (int j = startCol; j < startCol + 3; j++)
                        cells[i, j].BackColor = Color.LightYellow;

                cells[row, col].BackColor = Color.LightBlue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception in HighlightGroup: {ex.Message}\nrow={row}, col={col}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearHighlights()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var cell = cells[i, j];
                    if (!cell.Cell.IsInitial && (cell.Cell.DisplayedValue == null || cell.Cell.DisplayedValue != cell.Cell.RealValue))
                        cell.BackColor = Color.White;
                    else
                        cell.BackColor = Color.White;
                }
            }
        }
        private void StartTimer()
        {
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += (s, e) =>
            {
                timeElapsed = timeElapsed.Add(TimeSpan.FromSeconds(1));
                labelTimer.Text = timeElapsed.ToString("hh\\:mm\\:ss");
            };
            gameTimer.Start();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            checkAttempts++;
            bool isCorrect = true;

            foreach (var control in cells)
            {
                var cell = control.Cell;
                if (!cell.IsInitial && (cell.DisplayedValue == null || cell.DisplayedValue != cell.RealValue))
                {
                    control.BackColor = Color.Red;
                    isCorrect = false;
                }
            }

            if (isCorrect)
            {
                gameTimer.Stop();
                MessageBox.Show("Поздравляем!\nВремя: " + timeElapsed + "\nПопыток проверки: " + checkAttempts, "Решено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            timeElapsed = TimeSpan.Zero;
            labelTimer.Text = "00:00:00";
            checkAttempts = 0;
            InitializeBoard(lastSelectedDifficulty);
        }

        private Difficulty lastSelectedDifficulty = Difficulty.Medium;

        private void buttonDifficulty_Click(object sender, EventArgs e)
        {
            using (var dlg = new DifficultyDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    lastSelectedDifficulty = dlg.SelectedDifficulty;
                }
            }
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            foreach (var control in cells)
            {
                var cell = control.Cell;
                if (!cell.IsInitial)
                {
                    cell.DisplayedValue = cell.RealValue;
                    cell.Notes.Clear();
                    control.BackColor = Color.White;
                    control.UpdateDisplay();
                }
            }
        }
        
    }

    public partial class DifficultyDialog : Form
    {
        public Difficulty SelectedDifficulty { get; private set; } = Difficulty.Medium;
        private ComboBox comboBox;

        public DifficultyDialog()
        {
            Text = "Выбор сложности";
            Size = new Size(250, 100);
            StartPosition = FormStartPosition.CenterParent;

            comboBox = new ComboBox()
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBox.Items.AddRange(new object[] { "Легко", "Средне", "Сложно" });
            comboBox.SelectedIndex = 1;
            comboBox.SelectedIndexChanged += (s, e) =>
            {
                if (comboBox.SelectedIndex == 0) SelectedDifficulty = Difficulty.Easy;
                else if (comboBox.SelectedIndex == 1) SelectedDifficulty = Difficulty.Medium;
                else if (comboBox.SelectedIndex == 2) SelectedDifficulty = Difficulty.Hard;
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(comboBox);
        }
    }
}


