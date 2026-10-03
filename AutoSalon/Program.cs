namespace AutoSalonApp
{
    internal static class Program
    {
        /// <summary>Точка входа: запускается титульная форма.</summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new TitleForm());
        }
    }
}
