using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace IndividualProject
{
    // Map the class Trainer to the corresponding database table
    [Table(Name = "Trainer")]
    class Trainer
    {
        // Designating properties to represent the corresponding table columns in database
        [Column(Name = "ID")]
        public int ID { get; set; }
        [Column(Name = "FName")]
        public string FirstName { get; set; }
        [Column(Name = "LName")]
        public string LastName { get; set; }
        [Column(Name = "Subject")]
        public string Subject { get; set; }
        [Column(Name = "CourseID")]
        public int CourseID { get; set; }


        // Create trainers and store them in the database
        public static string CreateTrainer()
        {
            // Insert trainer data from the console
            Console.Clear();
            Console.WriteLine("\n- Trainers Data Creation\n");
            Console.Write("FirstName: ");
            string firstname = Console.ReadLine();
            Console.Write("LastName: ");
            string lastname = Console.ReadLine();
            Console.Write("Subject: ");
            string subject = Console.ReadLine();

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert a trainer
            // Define the type of command as a Store Procedure
            SqlCommand cmdInsert = new SqlCommand("spTrainerCRUD", db.SqlConnection);
            cmdInsert.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdInsert.Parameters.Add(new SqlParameter("@FName", firstname));
            cmdInsert.Parameters.Add(new SqlParameter("@LName", lastname));
            cmdInsert.Parameters.Add(new SqlParameter("@Subject", subject));

            int courseId = 0; // the FK CourseID
            // Call of method to check if the FK is reference the PK table Course
            courseId = db.CheckForeignKey(courseId, "Trainer");

            // Store Procedure parameters: The CourseID can now be commited
            cmdInsert.Parameters.Add(new SqlParameter("@CourseId", courseId));
            cmdInsert.Parameters.Add(new SqlParameter("@StatementType", "INSERT"));

            // Check the number of rows affected
            int insertedRows = cmdInsert.ExecuteNonQuery();
            // And print the appropriate message
            string message = insertedRows > 0 ? "\nInsert Success. " + $"{insertedRows} Row(s) commited successfully."
                + "\nPress any key to continue..." : "\nInsert Failed.\n";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Select all the trainers from the database
        public static string ReadTrainer()
        {
            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert a trainer
            // Define the type of command as a Store Procedure
            SqlCommand cmdSelect = new SqlCommand("spTrainerCRUD", db.SqlConnection);
            cmdSelect.CommandType = CommandType.StoredProcedure;
            cmdSelect.Parameters.Add(new SqlParameter("@StatementType", "SELECT"));

            // Create a reader to retrieve the rows with all the trainers from the database
            SqlDataReader readerTrainers = cmdSelect.ExecuteReader();
            // A list to store the retrieved rows of all the trainers
            List<Trainer> trainers = new List<Trainer>();

            // While there are rows with trainers, the reader reads
            while (readerTrainers.Read())
            {
                // Instantiate a new trainer and initialize its properties
                // with the appropriate values from the database
                Trainer trainer = new Trainer
                {
                    ID = readerTrainers.GetInt32(0),
                    FirstName = readerTrainers.GetString(1),
                    LastName = readerTrainers.GetString(2),
                    Subject = readerTrainers.GetString(3),
                    CourseID = readerTrainers.GetInt32(4)
                };
                // Adds the new trainer in the list with trainers
                trainers.Add(trainer);
            }

            // Iterate the list with trainers and print their data
            Console.Clear();
            Console.WriteLine("\n- Trainers Data Retrieval\n");

            foreach (Trainer trainer in trainers)
            {
                Console.WriteLine(trainer.ToString());
            }
            string message = "\nPress any key to continue...";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Update trainer data in database table
        public static string UpdateTrainer()
        {
            // Local variables declaration
            int trainerId = 0, courseId = 0, menuChoice = 0; // PK, FK
            string firstname = string.Empty, lastname = string.Empty, subject = string.Empty;
            string fullnameAsSpParam = string.Empty; // Passed as a parameter in the Store Procedure
            bool choiceIsValid = true, trainerIsFound = false, trainerIsUpdated = false; // Control flow

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Establish a connection between code-based data structures and the
            // database itself to retrieve objects (from db) and submit changes
            DataContext dataContext = new DataContext(db.SqlConnection);
            // Act as the logical, typed table for the queries against table Trainer in the database
            Table<Trainer> trainers = dataContext.GetTable<Trainer>();

            // Create the SQL command to update a trainer
            // Define the type of command as a Store Procedure
            SqlCommand cmdUpdate = new SqlCommand("spTrainerCRUD", db.SqlConnection);
            cmdUpdate.CommandType = CommandType.StoredProcedure;

            // Search for a trainer to update data based on parameters
            do
            {
                Console.Clear();
                Console.WriteLine("\n- Trainers Data Update\n");
                Console.WriteLine("Select how you want to search for a Trainer\n");
                Console.WriteLine("1. Search by ID");
                Console.WriteLine("2. Search by FullName");
                Console.Write("\nEnter your preference: ");
                menuChoice = int.Parse(Console.ReadLine());
                choiceIsValid = true;

                switch (menuChoice)
                {
                    case 1:
                        Console.Write("\nID: ");
                        trainerId = int.Parse(Console.ReadLine());

                        // In order to search for a trainer based on his/her ID
                        // we need to pass it as a parameter in the Store Procedure
                        cmdUpdate.Parameters.Add(new SqlParameter("@Id", trainerId));

                        // LINQ Lambda expression:
                        // Filter all the ids from table Trainer and get the one requested
                        var id = trainers.Where(t => t.ID.Equals(trainerId));
                        // If id exists in table Trainer
                        trainerIsFound = id.Any() ? true : false;
                        break;

                    case 2:
                        Console.Write("\nFirstName: ");
                        firstname = Console.ReadLine();
                        Console.Write("LastName: ");
                        lastname = Console.ReadLine();

                        // LINQ Lambda expression:
                        // Filter all the firstnames and lastnames from table Trainer and get the ones requested
                        var queryFullname = trainers.Where(t => t.FirstName.Equals(firstname) && t.LastName.Equals(lastname));

                        // Concatenate the Firstname and Lastname as Fullname
                        fullnameAsSpParam = string.Concat(firstname, lastname);
                        // If fullname exists in table Trainer
                        trainerIsFound = queryFullname.Any() ? true : false;
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        choiceIsValid = false;
                        break;
                }
            } while (!choiceIsValid);

            // Update trainer's data from the console
            while (choiceIsValid && trainerIsFound && !trainerIsUpdated)
            {
                // Get the trainer that was found either by id or by fullname
                var selectedTrainer =
                trainers.Where(t => t.ID == trainerId || t.FirstName == firstname && t.LastName == lastname);

                // Print all the selected trainer's data
                if (selectedTrainer.Any())
                {
                    Console.Clear();
                    foreach (var trainer in selectedTrainer)
                    {
                        Console.WriteLine($"\nTrainer exists in database: {trainer.ToString()}\n");
                    }
                }

                Console.WriteLine("\nSelect data of Trainer to update\n");
                Console.WriteLine("1. Firstname");
                Console.WriteLine("2. Lastname");
                Console.WriteLine("3. Subject");
                Console.WriteLine("4. Course ID");
                Console.Write("\nEnter your preference: ");
                menuChoice = int.Parse(Console.ReadLine());
                trainerIsUpdated = true;

                // Each variable is passed as a parameter in the
                // Store Procedure to update the corresponding field
                switch (menuChoice)
                {
                    case 1:
                        Console.Write("FirstName: ");
                        firstname = Console.ReadLine();
                        cmdUpdate.Parameters.Add(new SqlParameter("@FName", firstname));
                        break;

                    case 2:
                        Console.Write("LastName: ");
                        lastname = Console.ReadLine();
                        cmdUpdate.Parameters.Add(new SqlParameter("@LName", lastname));
                        break;

                    case 3:
                        Console.Write("Subject: ");
                        subject = Console.ReadLine();
                        cmdUpdate.Parameters.Add(new SqlParameter("@Subject", subject));
                        break;

                    case 4:
                        // Call of method to check if the FK is reference the PK table Course
                        courseId = db.CheckForeignKey(courseId, "Trainer");
                        cmdUpdate.Parameters.Add(new SqlParameter("@CourseId", courseId));
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        trainerIsUpdated = false;
                        break;
                }
            }

            // Store Procedure parameters
            cmdUpdate.Parameters.Add(new SqlParameter("@FullnameAsParam", fullnameAsSpParam));
            cmdUpdate.Parameters.Add(new SqlParameter("@StatementType", "UPDATE"));

            // Check the number of rows affected
            int updateRows = cmdUpdate.ExecuteNonQuery();
            // And print the appropriate message
            string message = updateRows > 0 ? "\nUpdate Success. " + $"{updateRows} Row(s) updated successfully."
                + "\nPress any key to continue..." : "\nNon-existent Primary Key or Fullname. Update Failed."
                + "\nPress any key to return to the CRUD menu...";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Delete a trainer from the database
        public static string DeleteTrainer()
        {
            // Local variables declaration
            int trainerId = 0;
            string firstname = string.Empty, lastname = string.Empty;
            bool selectionIsValid; // Control flow

            // The available parameters to delete a trainer
            do
            {
                // Insert trainer data from the console
                Console.Clear();
                Console.WriteLine("\n- Trainers Data Deletion\n");
                Console.WriteLine("Select how you want to search for a Trainer\n");
                Console.WriteLine("1. Search by ID");
                Console.WriteLine("2. Search by FullName");

                Console.Write("\nEnter your preference: ");
                int menuChoice = int.Parse(Console.ReadLine());
                selectionIsValid = true;

                switch (menuChoice)
                {
                    case 1:
                        Console.Write("\nID: ");
                        trainerId = int.Parse(Console.ReadLine());
                        break;

                    case 2:
                        Console.Write("\nFirstName: ");
                        firstname = Console.ReadLine();
                        Console.Write("LastName: ");
                        lastname = Console.ReadLine();
                        break;

                    default:
                        Console.Write("\nWrong input. Press any key to try again...");
                        Console.ReadKey();
                        selectionIsValid = false;
                        break;
                }
            } while (!selectionIsValid);

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to delete a trainer
            // Define the type of command as a Store Procedure
            SqlCommand cmdDelete = new SqlCommand("spTrainerCRUD", db.SqlConnection);
            cmdDelete.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdDelete.Parameters.Add(new SqlParameter("@Id", trainerId));
            cmdDelete.Parameters.Add(new SqlParameter("@FName", firstname));
            cmdDelete.Parameters.Add(new SqlParameter("@LName", lastname));
            cmdDelete.Parameters.Add(new SqlParameter("@StatementType", "DELETE"));

            // Check the number of rows affected
            int deletedRows = cmdDelete.ExecuteNonQuery();
            // And print the appropriate message
            string message = deletedRows > 0 ? "\nDeletion Success. " + $"{deletedRows} Row(s) erased successfully."
                + "\nPress any key to continue..." : "\nNon-existent Primary Key or Fullname. Deletion Failed."
                + "\nPress any key to return to the CRUD menu...";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // TODO: Print aligned columns
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb
                .Append($"ID: {ID}" + "|")
                .Append($"First Name: {FirstName}" + "|")
                .Append($"Last Name: {LastName}" + "|")
                .Append($"Subject: {Subject}" + "|")
                .Append($"Course ID: {CourseID}");
            return sb.ToString();
        }

    }
}
