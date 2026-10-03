namespace AutoSalonApp
{
    /// <summary>Менеджер: сотрудник, у которого есть бонус. Зарплата = оклад + бонус.</summary>
    public class Manager : Employee
    {
        private double bonus;                // бонус менеджера (накапливается с продаж)

        // Конструктор 1: бонуса нет
        public Manager(string name, double salary) : base(name, salary)
        {
            bonus = 0;
        }

        // Конструктор 2: бонус задан
        public Manager(string name, double salary, double bonus) : base(name, salary)
        {
            this.bonus = bonus;
        }

        public double GetBonus() { return bonus; }
        public void SetBonus(double bonus) { this.bonus = bonus; }

        // Переопределение: зарплата менеджера = оклад + бонус
        public override double GetSalary()
        {
            return base.GetSalary() + bonus;
        }

        // Строка вида: "id менеджер ФИО зарплата(бонус)"
        public override string ToString()
        {
            return getId() + " менеджер " + GetName() + " " + GetSalary().ToString("N0")
                   + " (бонус " + bonus.ToString("N0") + ")";
        }
    }
}
