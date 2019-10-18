using System;

namespace IndividualProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a User Interface
            UserInterface ui1 = new UserInterface();

            // Call the Login Console to proceed with user authedication
            int userRole = ui1.LoginConsole();

            // Different access is granted depending on user permissions
            ui1.DisplayMainMenu(userRole);
        }
    }
}
