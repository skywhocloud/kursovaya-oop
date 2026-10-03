using System.Globalization;
using System.Text;

namespace AutoSalonApp
{
    /// <summary>Автосалон: хранит списки автомобилей, сотрудников и продаж, содержит бизнес-логику.</summary>
    public class Salon
    {
        public const double BonusRate = 0.01;           // доля от цены, которая идёт менеджеру в бонус (1%)

        public List<Car> Cars = new List<Car>();        // автомобили
        public List<Employee> Staff = new List<Employee>(); // сотрудники и менеджеры
        public List<Sale> Sales = new List<Sale>();     // продажи

        // Оформление продажи: помечает автомобиль проданным и начисляет бонус менеджеру
        public void RegisterSale(Car car, Employee seller, DateTime date)
        {
            car.IsSold = true;
            Sales.Add(new Sale(car, seller, date, car.Price));

            // Полиморфизм/приведение типов: бонус получает только менеджер
            if (seller is Manager m)
            {
                m.SetBonus(m.GetBonus() + car.Price * BonusRate);
            }
        }

        // Общая сумма всех продаж
        public double TotalRevenue()
        {
            double sum = 0;
            foreach (Sale s in Sales)
            {
                sum += s.Amount;
            }
            return sum;
        }

        // Число автомобилей, доступных для продажи
        public int CountAvailable()
        {
            int count = 0;
            foreach (Car c in Cars)
            {
                if (!c.IsSold) count++;
            }
            return count;
        }

        // Демонстрационные данные (используются при первом запуске)
        public void FillDemo()
        {
            Cars.Add(new Car("Toyota", "Camry", 2023, 3200000));
            Cars.Add(new Car("Kia", "Rio", 2022, 1450000));
            Cars.Add(new Car("Skoda", "Octavia", 2024, 2600000));
            Cars.Add(new Car("BMW", "X3", 2023, 6100000));
            Cars.Add(new Car("Lada", "Vesta", 2024, 1300000));

            Staff.Add(new Employee("Петров Пётр Сергеевич", 40000));
            Staff.Add(new Employee("Козлова Анна Викторовна", 45000));
            Staff.Add(new Manager("Сидорова Мария Ивановна", 60000));
            Staff.Add(new Manager("Кузнецов Дмитрий Андреевич", 70000, 20000));
        }

        // ---------- Работа с файлом ----------

        // Символ-разделитель '|' в текстах заменяем пробелом, чтобы не ломать формат файла
        private static string Clean(string s)
        {
            return s.Replace('|', ' ');
        }

        // Число в текст независимо от региональных настроек
        private static string Num(double v)
        {
            return v.ToString(CultureInfo.InvariantCulture);
        }

        // Сохранение данных в текстовый файл
        public void Save(string path)
        {
            List<string> lines = new List<string>();

            foreach (Car c in Cars)
            {
                lines.Add("C|" + c.Id + "|" + Clean(c.Brand) + "|" + Clean(c.Model) + "|" +
                          c.Year + "|" + Num(c.Price) + "|" + (c.IsSold ? 1 : 0));
            }
            foreach (Employee e in Staff)
            {
                if (e is Manager m)
                    lines.Add("M|" + m.getId() + "|" + Clean(m.GetName()) + "|" +
                              Num(m.GetBaseSalary()) + "|" + Num(m.GetBonus()));
                else
                    lines.Add("E|" + e.getId() + "|" + Clean(e.GetName()) + "|" +
                              Num(e.GetBaseSalary()));
            }
            foreach (Sale s in Sales)
            {
                lines.Add("S|" + s.Car.Id + "|" + s.Seller.getId() + "|" +
                          s.Date.ToString("yyyy-MM-dd") + "|" + Num(s.Amount));
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        // Загрузка данных из файла. Возвращает false, если файла нет
        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;

            Cars.Clear();
            Staff.Clear();
            Sales.Clear();

            int maxCarId = 0;
            int maxEmpId = 0;
            CultureInfo inv = CultureInfo.InvariantCulture;

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                string[] p = line.Split('|');
                if (p.Length < 2) continue;

                if (p[0] == "C" && p.Length >= 7)
                {
                    Car c = new Car(p[2], p[3], int.Parse(p[4]), double.Parse(p[5], inv));
                    c.SetId(int.Parse(p[1]));
                    c.IsSold = p[6] == "1";
                    Cars.Add(c);
                    if (c.Id > maxCarId) maxCarId = c.Id;
                }
                else if (p[0] == "E" && p.Length >= 4)
                {
                    Employee e = new Employee(p[2], double.Parse(p[3], inv));
                    e.setId(int.Parse(p[1]));
                    Staff.Add(e);
                    if (e.getId() > maxEmpId) maxEmpId = e.getId();
                }
                else if (p[0] == "M" && p.Length >= 5)
                {
                    Manager m = new Manager(p[2], double.Parse(p[3], inv), double.Parse(p[4], inv));
                    m.setId(int.Parse(p[1]));
                    Staff.Add(m);
                    if (m.getId() > maxEmpId) maxEmpId = m.getId();
                }
                else if (p[0] == "S" && p.Length >= 5)
                {
                    int carId = int.Parse(p[1]);
                    int empId = int.Parse(p[2]);
                    Car car = Cars.Find(x => x.Id == carId);
                    Employee emp = Staff.Find(x => x.getId() == empId);
                    if (car != null && emp != null)
                    {
                        DateTime date = DateTime.ParseExact(p[3], "yyyy-MM-dd", inv);
                        Sales.Add(new Sale(car, emp, date, double.Parse(p[4], inv)));
                    }
                }
            }

            // Следующие номера должны быть больше уже существующих
            Car.SetNextId(maxCarId + 1);
            Employee.SetNextId(maxEmpId + 1);
            return true;
        }
    }
}
