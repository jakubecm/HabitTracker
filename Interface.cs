

namespace HabitTracker
{
    public class Interface
    {
        public void PresentMenu()
        {
            Console.WriteLine("MAIN MENU - HABIT LOGGER\n");
            Console.WriteLine("What would you like to do?");

            Console.Write(@"
            Type 0 to Close Application.
            Type 1 to View All Records.
            Type 2 to Insert a new Record.
            Type 3 to Delete a Record.
            Type 4 to Update a Record
            Type 5 to Create a new Habit");
            Console.WriteLine("\n--------------------------------------------");
        }

        public static int ParseSelection()
        {
            Console.Write("Your Selection:  ");
            bool valid = false;
            int parsedInput = 0;

            while (!valid)
            {
                string? userInput = Console.ReadLine();
                valid = int.TryParse(userInput, out parsedInput);

                if (!valid)
                {
                    Console.WriteLine("Invalid input, please try again and input a valid number.");
                    Console.Write("Your Selection:  ");
                }
            }

            return parsedInput;
        }

        internal bool ExecuteSelected(int selectedOption, DatabaseController dbController)
        {
            switch (selectedOption)
            {
                case 0:
                    return true;
                case 1:
                    dbController.ViewRecords();
                    return false;
                case 2:
                    dbController.InsertRecord();
                    return false;
                case 3:
                    dbController.DeleteRecord();
                    return false;
                case 4:
                    dbController.UpdateRecord();
                    return false;
                case 5:
                    dbController.CreateHabit();
                    return false;
                default:
                    Console.WriteLine("Invalid input detected. Please retry.");
                    Console.ReadKey();
                    return false;
            }
        }
    }
}
