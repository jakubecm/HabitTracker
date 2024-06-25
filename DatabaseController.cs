using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitTracker
{
    public class DatabaseController
    {
        private string connectionString;
        private SqliteConnection connection;
        private List<(string habitName, string unitName)> habits;

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

        internal void PrepareInsert()
        {
            string date = DateTime.Now.ToString("dd-MM-yyyy");

            Console.WriteLine("Which habit would you like to log?");
            int selectedOption = SelectHabitFromDb();

            Console.Write("Do you want to use today's date? (y/n):   ");
            var userResponse = Console.ReadKey().ToString();

            while (userResponse != "y" && userResponse != "n")
            {
                Console.WriteLine("Invalid input, please reenter:");
                Console.Write("Do you want to use today's date? (y/n):  ");
                userResponse = Console.ReadKey().ToString();
            }

            if (userResponse == "n")
            {
                bool isValid = false;

                while (!isValid)
                {
                    Console.Write("Enter your own date in the format DD-MM-YYYY:  ");
                    date = Console.ReadLine()!;
                    isValid = IsValidDate(date!);
                }
            }

            Console.Write($"Enter amount of {this.habits[selectedOption].unitName}:  ");
            int measure = Interface.ParseSelection();

            InsertRecord(this.habits[selectedOption], date, measure);
        }

        internal void InsertRecord((string tableName, string unitsName) habit, string date, int measure)
        {
            this.connection.Open();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = $"INSERT INTO {habit.tableName} (Date, {habit.unitsName}) VALUES (@date, @measure)";
            insertCmd.Parameters.AddWithValue("@date", date);
            insertCmd.Parameters.AddWithValue("@measure", measure);

            insertCmd.ExecuteNonQuery();
            this.connection.Close();
        }

        internal void ViewRecords()
        {
            Console.WriteLine("Which habit records would you like to see?");
            int selectedOption = SelectHabitFromDb();
            string tableName = this.habits[selectedOption].habitName;

            var viewString = $"SELECT * FROM {tableName}";

            try
            {
                connection.Open();
                using var command = new SqliteCommand(viewString, connection);
                using var reader = command.ExecuteReader();

                Console.WriteLine($"HABIT RECORDS :  {tableName}");
                Console.Write("----------------------------------");

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var date = reader.GetString(1);
                        var unit = reader.GetString(2);
                        Console.WriteLine($"{id}\t{date}\t{unit}");
                    }
                }
                else
                {
                    Console.WriteLine("No records found.");
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal void UpdateRecord()
        {
            ViewHabits();
            Console.WriteLine("Which habit table do you wish to update?");
            var selectedHabitIndex = SelectHabitFromDb();
            var table = this.habits[selectedHabitIndex].habitName;
            var unit = this.habits[selectedHabitIndex].unitName;

            Console.WriteLine("What is the ID of the record you wish to update?");
            var selectedRecordId = Interface.ParseSelection();

            bool recordExists = DoesRecordExist(table, selectedRecordId);

            if (recordExists)
            {
                Console.Write("Insert the updated measure for this record: ");
                var updatedMeasure = Interface.ParseSelection();

                try
                {
                    connection.Open();
                    var updateQuery = connection.CreateCommand();
                    updateQuery.CommandText = $"UPDATE {table} SET {unit} = @{unit} WHERE Id = @id";
                    updateQuery.Parameters.AddWithValue($"@{unit}", updatedMeasure);
                    updateQuery.Parameters.AddWithValue($"@Id", selectedRecordId);

                    updateQuery.ExecuteNonQuery();
                    Console.WriteLine($"Row updated sucessfuly. Press any key to continue.");
                    Console.ReadKey();
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("There is no record of the entered ID in the selected habit table.");
            }
        }

        internal void DeleteRecord()
        {
            ViewHabits();
            Console.WriteLine("Which habit table do you wish to delete from?");
            var selectedHabitIndex = SelectHabitFromDb();
            var table = this.habits[selectedHabitIndex].habitName;
            var unit = this.habits[selectedHabitIndex].unitName;

            Console.WriteLine("What is the ID of the record you wish to delete?");
            var selectedRecordId = Interface.ParseSelection();

            bool recordExists = DoesRecordExist(table, selectedRecordId);

            if (recordExists)
            {
                try
                {
                    connection.Open();
                    var updateQuery = connection.CreateCommand();
                    updateQuery.CommandText = $"DELETE FROM {table} WHERE Id = @id";
                    updateQuery.Parameters.AddWithValue($"@Id", selectedRecordId);

                    updateQuery.ExecuteNonQuery();
                    Console.WriteLine($"Row deleted sucessfuly. Press any key to continue.");
                    Console.ReadKey();
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("There is no record of the entered ID in the selected habit table.");
            }
        }

        internal void CreateHabit()
        {
            Console.Write("Insert a name for the new habit: ");
            var habitName = Console.ReadLine();

            Console.Write("\nInsert the unit of measurement of this habit (eg. 'Glasses' (of water each day)):  ");
            var unitName = Console.ReadLine();

            this.connection.Open();
            var createCmd = connection.CreateCommand();
            createCmd.CommandText = $"CREATE TABLE IF NOT EXISTS {habitName} (Id INTEGER PRIMARY KEY AUTOINCREMENT, Date TEXT, {unitName} INTEGER";
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
        internal int SelectHabitFromDb()
        {
            bool validChoice = false;
            int selectedOption = 0;
            ViewHabits();
            Console.Write("Your choice: ");

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

            return selectedOption;
        }

        internal static bool IsValidDate(string date)
        {
            DateTime tempDate;
            string format = "dd-MM-yyyy";
            CultureInfo provider = CultureInfo.InvariantCulture;

            return DateTime.TryParseExact(date, format, provider, DateTimeStyles.None, out tempDate);
        }

        internal bool DoesRecordExist(string table, int id)
        {
            connection.Open();

            var existanceQuery = connection.CreateCommand();
            existanceQuery.CommandText = $"SELECT * FROM {table} WHERE Id = {id}";
            var foundRecord = existanceQuery.ExecuteScalar();

            connection.Close();

            if (foundRecord != null)
            {
                return true;
            }

            return false;
        }

    }
}
