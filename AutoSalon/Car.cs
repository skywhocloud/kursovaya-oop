namespace AutoSalonApp
{
    /// <summary>Автомобиль, находящийся в салоне.</summary>
    public class Car
    {
        private static int nextId = 1;       // статическое поле: номер для следующего автомобиля

        public int Id { get; private set; }  // уникальный номер автомобиля
        public string Brand { get; set; }    // марка
        public string Model { get; set; }    // модель
        public int Year { get; set; }        // год выпуска
        public double Price { get; set; }    // цена, руб.
        public bool IsSold { get; set; }     // признак: автомобиль продан

        public Car(string brand, string model, int year, double price)
        {
            Id = nextId;
            nextId++;
            Brand = brand;
            Model = model;
            Year = year;
            Price = price;
            IsSold = false;
        }

        public void SetId(int id) { Id = id; }
        public static void SetNextId(int value) { nextId = value; }

        public override string ToString()
        {
            return Brand + " " + Model + ", " + Year + " г., " + Price.ToString("N0") + " руб.";
        }
    }
}
