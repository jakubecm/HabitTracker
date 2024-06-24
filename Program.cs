namespace HabitTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool requiresClosing = false;

            Interface appInterface = new Interface();
            DatabaseController dbController = new(@"Data Source=habit-tracker.db");

            dbController.InitializeDatabase();

            while (!requiresClosing)
            {
                Console.Clear();
                appInterface.PresentMenu();
                var selectedOption = Interface.ParseSelection();
                requiresClosing = appInterface.ExecuteSelected(selectedOption, dbController);
            }
        }
    }


}
