namespace AutoSalonApp
{
    /// <summary>Титульная форма: сведения о курсовой работе. Открывается первой.</summary>
    public class TitleForm : Form
    {
        // Данные для титульной формы (при необходимости измените)
        private const string University = "Государственное бюджетное образовательное учреждение высшего образования Московской области «Университет «Дубна»";
        private const string Department = "Кафедра системного анализа и управления";
        private const string Supervisor = "ст. преп. Беднякова Т.М., ст. преп. Арефьева А.В.";

        private int currentTop = 25;         // текущая вертикальная позиция при добавлении надписей

        public TitleForm()
        {
            Text = "Титульная форма";
            ClientSize = new Size(700, 600);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            AddLine(University, 11, true, 5);
            AddLine(Department, 11, true, 20);
            AddLine("КУРСОВАЯ РАБОТА", 18, true, 20);
            AddLine("по объектно-ориентированному программированию", 12, false, 20);
            AddLine("на тему:", 12, false, 5);
            AddLine("«Разработка программного обеспечения для менеджера автосалона»", 15, true, 30);
            AddLine("Студент: Смолев Глеб Дмитриевич", 12, false, 5);
            AddLine("Группа: Прог С-25, 2 курс", 12, false, 5);
                        AddLine("Руководители: " + Supervisor, 12, false, 30);
            AddLine("Дубна, 2026", 12, false, 25);

            Button btnStart = new Button();
            btnStart.Text = "Начать работу";
            btnStart.Size = new Size(180, 40);
            btnStart.Location = new Point((ClientSize.Width - 180) / 2, currentTop);
            btnStart.Click += btnStart_Click;
            Controls.Add(btnStart);

            // высота окна подстраивается под содержимое
            ClientSize = new Size(700, btnStart.Bottom + 30);
        }

        // Добавляет центрированную надпись и сдвигает позицию вниз
        private void AddLine(string text, float size, bool bold, int gap)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.AutoSize = false;
            lbl.Width = ClientSize.Width - 40;
            lbl.Height = (int)(size * 2.4) + (text.Length > 55 ? 28 : 0);
            lbl.Location = new Point(20, currentTop);
            Controls.Add(lbl);
            currentTop += lbl.Height + gap;
        }

        // Переход на рабочую форму
        private void btnStart_Click(object sender, EventArgs e)
        {
            MainForm main = new MainForm();
            main.FormClosed += (s, args) => Close();   // закрытие рабочей формы завершает программу
            Hide();
            main.Show();
        }
    }
}
