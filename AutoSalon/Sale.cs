namespace AutoSalonApp
{
    /// <summary>Факт продажи автомобиля сотрудником.</summary>
    public class Sale
    {
        public Car Car { get; private set; }            // проданный автомобиль
        public Employee Seller { get; private set; }    // кто продал
        public DateTime Date { get; private set; }      // дата продажи
        public double Amount { get; private set; }      // сумма продажи, руб.

        public Sale(Car car, Employee seller, DateTime date, double amount)
        {
            Car = car;
            Seller = seller;
            Date = date;
            Amount = amount;
        }
    }
}
