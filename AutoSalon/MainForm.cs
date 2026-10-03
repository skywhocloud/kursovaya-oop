using System.Globalization;

namespace AutoSalonApp
{
    /// <summary>Рабочая форма: три вкладки (автомобили, сотрудники, продажи).</summary>
    public class MainForm : Form
    {
        private const string FileName = "autosalon.txt";   // файл с данными
        private Salon salon = new Salon();                 // модель данных автосалона

        // --- элементы вкладки "Автомобили" ---
        private TextBox txtBrand, txtModel, txtSearch;     // марка, модель, строка поиска
        private NumericUpDown numYear, numPrice;           // год выпуска и цена
        private DataGridView gridCars;                     // таблица автомобилей

        // --- элементы вкладки "Сотрудники" ---
        private RadioButton rbEmployee, rbManager;         // выбор типа сотрудника
        private TextBox txtName, txtSalary, txtBonus, txtPercent; // ФИО, оклад, бонус, процент
        private ListBox lstEmployees, lstManagers;         // отдельные списки

        // --- элементы вкладки "Продажи" ---
        private ComboBox cmbCar, cmbSeller;                // выбор автомобиля и продавца
        private DateTimePicker dtpDate;                    // дата продажи
        private DataGridView gridSales;                    // таблица продаж
        private Label lblTotal;                            // итоговая сумма

        public MainForm()
        {
            Text = "Автосалон — рабочая форма";
            ClientSize = new Size(960, 620);
            StartPosition = FormStartPosition.CenterScreen;

            BuildInterface();
            LoadData();
            RefreshAll();

            FormClosing += (s, e) => SaveData();           // сохраняем данные при закрытии
        }

        // ================= Построение интерфейса =================

        private void BuildInterface()
        {
            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.TabPages.Add(BuildCarsPage());
            tabs.TabPages.Add(BuildStaffPage());
            tabs.TabPages.Add(BuildSalesPage());
            Controls.Add(tabs);
        }

        // Вспомогательные методы создания элементов
        private Label MakeLabel(string text, int x, int y)
        {
            Label l = new Label();
            l.Text = text;
            l.AutoSize = true;
            l.Location = new Point(x, y);
            return l;
        }

        private TextBox MakeText(int x, int y, int w)
        {
            TextBox t = new TextBox();
            t.Location = new Point(x, y);
            t.Width = w;
            return t;
        }

        private Button MakeButton(string text, int x, int y, int w, EventHandler handler)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(w, 28);
            b.Click += handler;
            return b;
        }

        private DataGridView MakeGrid(int x, int y, int w, int h, params string[] columns)
        {
            DataGridView g = new DataGridView();
            g.Location = new Point(x, y);
            g.Size = new Size(w, h);
            g.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.RowHeadersVisible = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            foreach (string c in columns) g.Columns.Add(c, c);
            return g;
        }

        // ---- Вкладка "Автомобили" ----
        private TabPage BuildCarsPage()
        {
            TabPage page = new TabPage("Автомобили");

            GroupBox gb = new GroupBox();
            gb.Text = "Новый автомобиль";
            gb.Location = new Point(10, 10);
            gb.Size = new Size(920, 90);

            txtBrand = MakeText(10, 45, 150);
            txtModel = MakeText(180, 45, 150);
            numYear = new NumericUpDown();
            numYear.Location = new Point(350, 45);
            numYear.Width = 80;
            numYear.Minimum = 1980;
            numYear.Maximum = DateTime.Now.Year + 1;
            numYear.Value = DateTime.Now.Year;
            numPrice = new NumericUpDown();
            numPrice.Location = new Point(450, 45);
            numPrice.Width = 130;
            numPrice.Maximum = 100000000;
            numPrice.Increment = 50000;
            numPrice.ThousandsSeparator = true;

            gb.Controls.Add(MakeLabel("Марка", 10, 25));
            gb.Controls.Add(txtBrand);
            gb.Controls.Add(MakeLabel("Модель", 180, 25));
            gb.Controls.Add(txtModel);
            gb.Controls.Add(MakeLabel("Год выпуска", 350, 25));
            gb.Controls.Add(numYear);
            gb.Controls.Add(MakeLabel("Цена, руб.", 450, 25));
            gb.Controls.Add(numPrice);
            gb.Controls.Add(MakeButton("Добавить", 610, 42, 110, btnAddCar_Click));

            txtSearch = MakeText(130, 112, 170);
            txtSearch.TextChanged += (s, e) => RefreshCars();   // поиск обновляет таблицу сразу

            gridCars = MakeGrid(10, 145, 920, 400, "№", "Марка", "Модель", "Год", "Цена, руб.", "Статус");

            page.Controls.Add(gb);
            page.Controls.Add(MakeLabel("Поиск по марке:", 10, 115));
            page.Controls.Add(txtSearch);
            page.Controls.Add(MakeButton("Удалить выбранный", 320, 109, 160, btnDeleteCar_Click));
            page.Controls.Add(gridCars);
            return page;
        }

        // ---- Вкладка "Сотрудники" ----
        private TabPage BuildStaffPage()
        {
            TabPage page = new TabPage("Сотрудники");

            GroupBox gb = new GroupBox();
            gb.Text = "Новый сотрудник";
            gb.Location = new Point(10, 10);
            gb.Size = new Size(920, 120);

            rbEmployee = new RadioButton();
            rbEmployee.Text = "Сотрудник";
            rbEmployee.Location = new Point(10, 22);
            rbEmployee.Checked = true;
            rbManager = new RadioButton();
            rbManager.Text = "Менеджер";
            rbManager.Location = new Point(130, 22);
            rbManager.CheckedChanged += rbManager_CheckedChanged;

            txtName = MakeText(10, 80, 260);
            txtSalary = MakeText(290, 80, 110);
            txtBonus = MakeText(420, 80, 110);
            txtBonus.Enabled = false;                       // бонус доступен только для менеджера

            gb.Controls.Add(rbEmployee);
            gb.Controls.Add(rbManager);
            gb.Controls.Add(MakeLabel("ФИО", 10, 58));
            gb.Controls.Add(txtName);
            gb.Controls.Add(MakeLabel("Оклад, руб.", 290, 58));
            gb.Controls.Add(txtSalary);
            gb.Controls.Add(MakeLabel("Бонус, руб.", 420, 58));
            gb.Controls.Add(txtBonus);
            gb.Controls.Add(MakeButton("Добавить", 560, 77, 110, btnAddStaff_Click));

            lstEmployees = new ListBox();
            lstEmployees.Location = new Point(10, 160);
            lstEmployees.Size = new Size(450, 330);
            lstManagers = new ListBox();
            lstManagers.Location = new Point(480, 160);
            lstManagers.Size = new Size(450, 330);

            txtPercent = MakeText(250, 505, 60);

            page.Controls.Add(gb);
            page.Controls.Add(MakeLabel("Сотрудники", 10, 140));
            page.Controls.Add(MakeLabel("Менеджеры", 480, 140));
            page.Controls.Add(lstEmployees);
            page.Controls.Add(lstManagers);
            page.Controls.Add(MakeLabel("Повысить оклад выбранному на, %:", 10, 508));
            page.Controls.Add(txtPercent);
            page.Controls.Add(MakeButton("Повысить", 330, 502, 110, btnRaise_Click));
            return page;
        }

        // ---- Вкладка "Продажи" ----
        private TabPage BuildSalesPage()
        {
            TabPage page = new TabPage("Продажи");

            GroupBox gb = new GroupBox();
            gb.Text = "Оформление продажи";
            gb.Location = new Point(10, 10);
            gb.Size = new Size(920, 90);

            cmbCar = new ComboBox();
            cmbCar.Location = new Point(10, 45);
            cmbCar.Width = 340;
            cmbCar.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSeller = new ComboBox();
            cmbSeller.Location = new Point(370, 45);
            cmbSeller.Width = 260;
            cmbSeller.DropDownStyle = ComboBoxStyle.DropDownList;
            dtpDate = new DateTimePicker();
            dtpDate.Location = new Point(650, 45);
            dtpDate.Width = 110;
            dtpDate.Format = DateTimePickerFormat.Short;

            gb.Controls.Add(MakeLabel("Автомобиль (в наличии)", 10, 25));
            gb.Controls.Add(cmbCar);
            gb.Controls.Add(MakeLabel("Продавец", 370, 25));
            gb.Controls.Add(cmbSeller);
            gb.Controls.Add(MakeLabel("Дата", 650, 25));
            gb.Controls.Add(dtpDate);
            gb.Controls.Add(MakeButton("Продать", 780, 42, 120, btnSell_Click));

            gridSales = MakeGrid(10, 115, 920, 390, "Дата", "Автомобиль", "Продавец", "Сумма, руб.");

            lblTotal = MakeLabel("", 10, 520);
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTotal.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            page.Controls.Add(gb);
            page.Controls.Add(gridSales);
            page.Controls.Add(lblTotal);
            return page;
        }

        // ================= Обновление данных на форме =================

        private void RefreshAll()
        {
            RefreshCars();
            RefreshStaff();
            RefreshSales();
        }

        // Таблица автомобилей с учётом строки поиска
        private void RefreshCars()
        {
            gridCars.Rows.Clear();
            string filter = txtSearch.Text.Trim().ToLower();
            foreach (Car c in salon.Cars)
            {
                if (filter != "" && !c.Brand.ToLower().Contains(filter)) continue;
                int row = gridCars.Rows.Add(c.Id, c.Brand, c.Model, c.Year,
                                            c.Price.ToString("N0"), c.IsSold ? "Продан" : "В наличии");
                gridCars.Rows[row].Tag = c;                 // сохраняем объект в строке
            }
            RefreshCombos();
        }

        // Два отдельных списка: сотрудники и менеджеры
        private void RefreshStaff()
        {
            lstEmployees.Items.Clear();
            lstManagers.Items.Clear();
            foreach (Employee emp in salon.Staff)
            {
                if (emp is Manager) lstManagers.Items.Add(emp);
                else lstEmployees.Items.Add(emp);
            }
            RefreshCombos();
        }

        // Таблица продаж и итоги
        private void RefreshSales()
        {
            gridSales.Rows.Clear();
            foreach (Sale s in salon.Sales)
            {
                gridSales.Rows.Add(s.Date.ToString("dd.MM.yyyy"), s.Car.Brand + " " + s.Car.Model,
                                   s.Seller.GetName(), s.Amount.ToString("N0"));
            }
            lblTotal.Text = "Продаж: " + salon.Sales.Count + ", на сумму " +
                            salon.TotalRevenue().ToString("N0") + " руб. В наличии: " +
                            salon.CountAvailable();
        }

        // Выпадающие списки вкладки "Продажи"
        private void RefreshCombos()
        {
            if (cmbCar == null || cmbSeller == null) return;
            cmbCar.Items.Clear();
            foreach (Car c in salon.Cars)
            {
                if (!c.IsSold) cmbCar.Items.Add(c);
            }
            cmbSeller.Items.Clear();
            foreach (Employee emp in salon.Staff) cmbSeller.Items.Add(emp);
            if (cmbCar.Items.Count > 0) cmbCar.SelectedIndex = 0;
            if (cmbSeller.Items.Count > 0) cmbSeller.SelectedIndex = 0;
        }

        // ================= Обработчики событий =================

        private void btnAddCar_Click(object sender, EventArgs e)
        {
            string brand = txtBrand.Text.Trim();
            string model = txtModel.Text.Trim();
            if (brand == "" || model == "")
            {
                Warn("Введите марку и модель автомобиля");
                return;
            }
            if (numPrice.Value <= 0)
            {
                Warn("Цена должна быть больше нуля");
                return;
            }
            salon.Cars.Add(new Car(brand, model, (int)numYear.Value, (double)numPrice.Value));
            txtBrand.Clear();
            txtModel.Clear();
            RefreshCars();
        }

        private void btnDeleteCar_Click(object sender, EventArgs e)
        {
            if (gridCars.CurrentRow == null) { Warn("Выберите автомобиль в таблице"); return; }
            Car car = gridCars.CurrentRow.Tag as Car;
            if (car == null) return;
            if (car.IsSold)
            {
                Warn("Нельзя удалить проданный автомобиль");
                return;
            }
            salon.Cars.Remove(car);
            RefreshCars();
        }

        private void rbManager_CheckedChanged(object sender, EventArgs e)
        {
            txtBonus.Enabled = rbManager.Checked;           // бонус активен только у менеджера
            if (!rbManager.Checked) txtBonus.Clear();
        }

        private void btnAddStaff_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            double salary;
            double bonus = 0;

            if (name == "") { Warn("Введите ФИО"); return; }
            if (!TryReadNumber(txtSalary.Text, out salary)) { Warn("Введите корректный оклад"); return; }

            if (rbManager.Checked)
            {
                if (txtBonus.Text.Trim() == "")
                {
                    salon.Staff.Add(new Manager(name, salary));          // конструктор без бонуса
                }
                else if (TryReadNumber(txtBonus.Text, out bonus))
                {
                    salon.Staff.Add(new Manager(name, salary, bonus));   // конструктор с бонусом
                }
                else
                {
                    Warn("Введите корректный бонус");
                    return;
                }
            }
            else
            {
                salon.Staff.Add(new Employee(name, salary));
            }

            txtName.Clear();
            txtSalary.Clear();
            txtBonus.Clear();
            RefreshStaff();
        }

        private void btnRaise_Click(object sender, EventArgs e)
        {
            Employee emp = lstEmployees.SelectedItem as Employee;
            if (emp == null) emp = lstManagers.SelectedItem as Employee;
            if (emp == null) { Warn("Выберите сотрудника в одном из списков"); return; }

            double percent;
            if (!TryReadNumber(txtPercent.Text, out percent)) { Warn("Введите корректный процент"); return; }

            emp.IncreaseSalary(percent);
            RefreshStaff();
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            Car car = cmbCar.SelectedItem as Car;
            Employee seller = cmbSeller.SelectedItem as Employee;
            if (car == null) { Warn("Нет автомобиля для продажи"); return; }
            if (seller == null) { Warn("Выберите продавца"); return; }

            salon.RegisterSale(car, seller, dtpDate.Value.Date);
            RefreshAll();
        }

        // ================= Служебные методы =================

        private void Warn(string text)
        {
            MessageBox.Show(text, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Чтение неотрицательного числа (запятая и точка допустимы)
        private bool TryReadNumber(string text, out double value)
        {
            string fixedText = text.Trim().Replace(',', '.');
            bool ok = double.TryParse(fixedText, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            return ok && value >= 0;
        }

        // Загрузка из файла; при первом запуске подставляются демонстрационные данные
        private void LoadData()
        {
            try
            {
                if (!salon.Load(FileName)) salon.FillDemo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось прочитать файл данных: " + ex.Message);
                salon = new Salon();
                salon.FillDemo();
            }
        }

        // Сохранение в файл
        private void SaveData()
        {
            try
            {
                salon.Save(FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить данные: " + ex.Message);
            }
        }
    }
}
