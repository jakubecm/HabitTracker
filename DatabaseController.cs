using Microsoft.Data.Sqlite;

namespace HabitTracker
{
    public class DatabaseController
    {
        private string connectionString;

        public DatabaseController(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(this.connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Date TEXT,
                                            Quantity INTEGER
                                           )";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal void DeleteRecord()
        {
            throw new NotImplementedException();
        }

        internal void InsertRecord()
        {
            throw new NotImplementedException();
        }

        internal void UpdateRecord()
        {
            throw new NotImplementedException();
        }

        internal void ViewRecords()
        {
            throw new NotImplementedException();
        }
    }
}
