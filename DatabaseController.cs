using Microsoft.Data.Sqlite;

namespace HabitTracker
{
    public class DatabaseController
    {
        private string connectionString;
        private SqliteConnection connection;
        private List<(string, string)> habits;

        public DatabaseController(string connectionString)
        {
            this.connectionString = connectionString;
            this.connection = new SqliteConnection(connectionString);
            this.habits = new();
        }

        public void InitializeDatabase()
        {

            this.connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Date TEXT,
                                        Quantity INTEGER
                                        )";

            tableCmd.ExecuteNonQuery();

            this.connection.Close();


            Console.WriteLine("Database initialized with the initial habit: Drinking water. Press any key to continue.");
            Console.ReadKey();
        }

        internal void DeleteRecord()
        {
            throw new NotImplementedException();
        }

        internal void InsertRecord()
        {
            Console.WriteLine("Which habit would you like to log?");
            ViewHabits();
            Console.Write("Your choice: ");
            bool validChoice = false;
            int selectedOption = 0;

            while (!validChoice)
            {
                selectedOption = Interface.ParseSelection();

                if (selectedOption >= habits.Count || selectedOption < 0)
                {
                    Console.WriteLine("This is not a valid choice.");
                    continue;
                }

                validChoice = true;
            }

            Console.Write("Do you want to use today's date? (y/n):   ");
            var userResponse = Console.ReadKey();
        }

        internal void UpdateRecord()
        {
            throw new NotImplementedException();
        }

        internal void ViewRecords()
        {
            throw new NotImplementedException();
        }

        internal void CreateHabit()
        {
            Console.Write("Insert a name for the new habit: ");
            var habitName = Console.ReadLine();

            Console.Write("\nInsert the unit of measurement of this habit (eg. 'Glasses' (of water each day)):  ");
            var unitName = Console.ReadLine();

            this.connection.Open();
            var createCmd = connection.CreateCommand();
            createCmd.CommandText = $"CREATE TABLE IF NOT EXISTS {habitName} (ID INTEGER PRIMARY KEY AUTOINCREMENT, Date TEXT, {unitName} INTEGER";
            createCmd.ExecuteNonQuery();
            this.connection.Close();

            habits.Add((habitName!, unitName!));
        }

        internal void ViewHabits()
        {
            for (int i = 0; i < habits.Count; i++)
            {
                Console.WriteLine($"{i}: {habits[i].Item1}");

            }
            Console.WriteLine("-----------------------------");
        }
    }
}
