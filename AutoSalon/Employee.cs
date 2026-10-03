namespace AutoSalonApp
{
    /// <summary>Сотрудник автосалона (продавец-консультант). Базовый класс иерархии.</summary>
    public class Employee
    {
        private string name;                 // ФИО сотрудника
        private double salary;               // оклад (без бонуса)
        private int id;                      // уникальный номер сотрудника
        private static int nextId = 1;       // статическое поле: номер для следующего сотрудника

        // Конструктор 1: без параметров (значения по умолчанию)
        public Employee() : this("Не указано", 30000)
        {
        }

        // Конструктор 2: только ФИО, оклад по умолчанию
        public Employee(string name) : this(name, 30000)
        {
        }

        // Конструктор 3: ФИО и оклад
        public Employee(string name, double salary)
        {
            this.name = name;
            this.salary = salary;
            id = nextId;                     // присваиваем уникальный номер
            nextId++;
        }

        public string GetName() { return name; }
        public void SetName(string name) { this.name = name; }

        // Оклад без бонуса (нужен для сохранения в файл)
        public double GetBaseSalary() { return salary; }
        public void SetSalary(double salary) { this.salary = salary; }

        // Виртуальный метод: в классе Manager будет переопределён
        public virtual double GetSalary() { return salary; }

        // Повышение оклада на заданный процент
        public void IncreaseSalary(double percent)
        {
            salary = salary + salary * percent / 100;
        }

        public int getId() { return id; }
        public void setId(int id) { this.id = id; }

        public static int getNextId() { return nextId; }
        public static void SetNextId(int value) { nextId = value; }

        // Строка вида: "id сотрудник ФИО зарплата"
        public override string ToString()
        {
            return id + " сотрудник " + name + " " + GetSalary().ToString("N0");
        }
    }
}
