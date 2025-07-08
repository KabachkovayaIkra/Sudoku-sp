using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class UserControlCell : UserControl
    {
        public Cell Cell { get; private set; }
        private Label lblValue = new Label();
        private TableLayoutPanel notesPanel = new TableLayoutPanel();
        private Label[] noteLabels = new Label[9];

        public UserControlCell(Cell cell)
        {
            Cell = cell;
            this.Width = this.Height = 50;
            this.Margin = new Padding(0);
            this.BackColor = Color.White;

            // Разрешить выбор фокуса
            this.SetStyle(ControlStyles.Selectable, true);

            notesPanel.Dock = DockStyle.Fill;
            notesPanel.RowCount = 3;
            notesPanel.ColumnCount = 3;
            for (int i = 0; i < 3; i++)
            {
                notesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
                notesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            }

            for (int i = 0; i < 9; i++)
            {
                var lbl = new Label
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 6),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                noteLabels[i] = lbl;
                notesPanel.Controls.Add(lbl, i % 3, i / 3);

                // Обработка клика по заметке
                lbl.MouseDown += (s, e) => this.Focus();
            }

            lblValue.Dock = DockStyle.Fill;
            lblValue.Font = new Font("Segoe UI", 16);
            lblValue.TextAlign = ContentAlignment.MiddleCenter;

            // Обработка клика по значению
            lblValue.MouseDown += (s, e) => this.Focus();

            this.Controls.Add(notesPanel);
            this.Controls.Add(lblValue);

            UpdateDisplay();

            this.Click += UserControlCell_Click;
            this.MouseDown += (s, e) => this.Focus(); // Клик по самой ячейке
            this.KeyDown += UserControlCell_KeyDown;
            this.GotFocus += (s, e) => this.BackColor = Color.LightBlue;
            this.LostFocus += (s, e) => this.BackColor = Color.White;
            this.TabStop = true;
        }

        public void UpdateDisplay()
        {
            if (Cell.DisplayedValue.HasValue)
            {
                lblValue.Text = Cell.DisplayedValue.ToString();
                lblValue.Visible = true;
                notesPanel.Visible = false;
                lblValue.ForeColor = Cell.IsInitial ? Color.Black : Color.Blue;
            }
            else
            {
                lblValue.Visible = false;
                notesPanel.Visible = true;
                for (int i = 0; i < 9; i++)
                {
                    noteLabels[i].Text = Cell.Notes.Contains(i + 1) ? (i + 1).ToString() : "";
                }
            }
        }

        public void UserControlCell_Click(object sender, EventArgs e)
        {
            this.Focus();
        }

        public void UserControlCell_KeyDown(object sender, KeyEventArgs e)
        {
            if (Cell.IsInitial) return;

            if (e.Control && e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)
            {
                int note = e.KeyCode - Keys.D0;
                if (Cell.Notes.Contains(note)) Cell.Notes.Remove(note);
                else Cell.Notes.Add(note);
            }
            else if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)
            {
                Cell.DisplayedValue = e.KeyCode - Keys.D0;
                Cell.Notes.Clear();
                this.BackColor = Color.White;
            }
            else if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                Cell.DisplayedValue = null;
                Cell.Notes.Clear();
            }

            UpdateDisplay();
        }
    }
}
