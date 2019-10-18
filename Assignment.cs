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
    // Map the class Assignment to the corresponding database table
    [Table(Name = "Assignment")]
    class Assignment
    {
        // Designating properties to represent the corresponding table columns in database
        [Column(Name = "ID")]
        public int ID { get; set; }
        [Column(Name = "Title")]
        public string Title { get; set; }
        [Column(Name = "Description")]
        public string Description { get; set; }
        [Column(Name = "SubmissionDate")]
        public DateTime SubmissionDate { get; set; }
        [Column(Name = "OralMark")]
        public decimal OralMark { get; set; }
        [Column(Name = "TotalMark")]
        public decimal TotalMark { get; set; }
        [Column(Name = "CourseID")]
        public int CourseID { get; set; }


        // Create assignments and store them in the database
        public static string CreateAssignment()
        {
            // Insert assignment data from the console
            Console.Clear();
            Console.WriteLine("\n- Assignments Data Creation\n");
            Console.Write("Title: ");
            string title = Console.ReadLine();
            Console.Write("Description: ");
            string description = Console.ReadLine();
            Console.Write("Submission Date and Time: ");
            var submissionDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Oral Mark: ");
            decimal oralMark = decimal.Parse(Console.ReadLine());
            Console.Write("Total Mark: ");
            decimal totalMark = decimal.Parse(Console.ReadLine());

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert an assignment
            // Define the type of command as a Store Procedure
            SqlCommand cmdInsert = new SqlCommand("spAssignmentCRUD", db.SqlConnection);
            cmdInsert.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdInsert.Parameters.Add(new SqlParameter("@Title", title));
            cmdInsert.Parameters.Add(new SqlParameter("@Description", description));
            cmdInsert.Parameters.Add(new SqlParameter("@SubmissionDate", submissionDate));
            cmdInsert.Parameters.Add(new SqlParameter("@OralMark", oralMark));
            cmdInsert.Parameters.Add(new SqlParameter("@TotalMark", totalMark));

            int courseId = 0; // the FK CourseID
            // Call of method to check if the FK is reference the PK table Course
            courseId = db.CheckForeignKey(courseId, "Assignment");

            // Store Procedure parameters: The CourseID can now be commited
            cmdInsert.Parameters.Add(new SqlParameter("@CourseID", courseId));
            cmdInsert.Parameters.Add(new SqlParameter("@StatementType", "INSERT"));

            // Check the number of rows affected
            int insertedRows = cmdInsert.ExecuteNonQuery();
            // And print the appropriate message
            string message = insertedRows > 0 ? "\nInsert Success. " + $"{insertedRows} Row(s) inserted successfully."
               + "\nPress any key to continue..." : "\nInsert Failed.\n";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Select all the assignments from the database
        public static string ReadAssignment()
        {
            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert an assignment
            // Define the type of command as a Store Procedure
            SqlCommand cmdSelect = new SqlCommand("spAssignmentCRUD", db.SqlConnection);
            cmdSelect.CommandType = CommandType.StoredProcedure;
            cmdSelect.Parameters.Add(new SqlParameter("@StatementType", "SELECT"));

            // Create a reader to retrieve the rows with all the assignments from the database
            SqlDataReader readerAssignments = cmdSelect.ExecuteReader();
            // A list to store the retrieved rows of all the assignments
            List<Assignment> assignments = new List<Assignment>();

            // While there are rows with assignments, the reader reads
            while (readerAssignments.Read())
            {
                // Instantiate a new assignment and initialize its properties
                // with the appropriate values from the database
                Assignment assignment = new Assignment
                {
                    ID = readerAssignments.GetInt32(0),
                    Title = readerAssignments.GetString(1),
                    Description = readerAssignments.GetString(2),
                    SubmissionDate = readerAssignments.GetDateTime(3),
                    OralMark = readerAssignments.GetDecimal(5),
                    TotalMark = readerAssignments.GetDecimal(5),
                    CourseID = readerAssignments.GetInt32(6)
                };
                // Adds the new assignment in the list with Assignments
                assignments.Add(assignment);
            }

            // Iterate the list with assignments and print their data
            Console.Clear();
            Console.WriteLine("\n- Assignments Data Retrieval\n");

            foreach (Assignment assignment in assignments)
            {
                Console.WriteLine(assignment.ToString());
            }
            string message = "\nPress any key to continue...";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Update assignment data in database table
        public static string UpdateAssignment()
        {
            // Local variables declaration
            int assignmentId = 0, courseId = 0, menuChoice = 0; // PK, FK
            string title = string.Empty, description = string.Empty;
            DateTime submissionDate = DateTime.Now;
            decimal oralMark = 0, totalMark = 0;
            string titleAsSpParam = string.Empty; // Passed as a parameter in the Store Procedure
            bool choiceIsValid = true, assignmentIsFound = false, assignmentIsUpdated = false; // Control flow

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Establish a connection between code-based data structures and the
            // database itself to retrieve objects (from db) and submit changes
            DataContext dataContext = new DataContext(db.SqlConnection);
            // Act as the logical, typed table for the queries against table Assignment in the database
            Table<Assignment> assignments = dataContext.GetTable<Assignment>();

            // Create the SQL command to update an assignment
            // Define the type of command as a Store Procedure
            SqlCommand cmdUpdate = new SqlCommand("spAssignmentCRUD", db.SqlConnection);
            cmdUpdate.CommandType = CommandType.StoredProcedure;

            // Search for an assignment to update data based on parameters
            do
            {
                Console.Clear();
                Console.WriteLine("\n- Assignments Data Update\n");
                Console.WriteLine("Select how you want to search for an Assignment\n");
                Console.WriteLine("1. Search by ID");
                Console.WriteLine("2. Search by Title");
                Console.Write("\nEnter your preference: ");
                menuChoice = int.Parse(Console.ReadLine());
                choiceIsValid = true;

                switch (menuChoice)
                {
                    case 1:
                        Console.Write("\nID: ");
                        assignmentId = int.Parse(Console.ReadLine());

                        // In order to search for an assignment based on its ID
                        // we need to pass it as a parameter in the Store Procedure
                        cmdUpdate.Parameters.Add(new SqlParameter("@Id", assignmentId));

                        // LINQ Lambda expression:
                        // Filter all the ids from table Assignment and get the one requested
                        var id = assignments.Where(t => t.ID.Equals(assignmentId));
                        // If id exists in table Assignment
                        assignmentIsFound = id.Any() ? true : false;
                        break;

                    case 2:
                        Console.Write("\nTitle: ");
                        // Pass the course title as parameter in the Store Procedure
                        titleAsSpParam = title = Console.ReadLine();

                        // LINQ Lambda expression:
                        // Filter all the titles from table Assignment and get the one requested
                        var queryTitle = assignments.Where(a => a.Title.Equals(title));

                        // If title exists in table Assignment
                        assignmentIsFound = queryTitle.Any() ? true : false;
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        choiceIsValid = false;
                        break;
                }
            } while (!choiceIsValid);

            // Update assignment's data from the console
            while (choiceIsValid && assignmentIsFound && !assignmentIsUpdated)
            {
                // Get the assignment that was found either by id or by title
                var selectedAssingment =
                assignments.Where(t => t.ID == assignmentId || t.Title == title);

                // Print all the selected assignment's data
                if (selectedAssingment.Any())
                {
                    Console.Clear();
                    foreach (var assignment in selectedAssingment)
                    {
                        Console.WriteLine($"\nAssignment exists in database: {assignment.ToString()}\n");
                    }
                }

                Console.WriteLine("\nSelect data of Assignment to update\n");
                Console.WriteLine("1. Title");
                Console.WriteLine("2. Description");
                Console.WriteLine("3. Submission Date and Time");
                Console.WriteLine("4. Oral Mark");
                Console.WriteLine("5. Total Mark");
                Console.WriteLine("6. Course ID");
                Console.Write("\nEnter your preference: ");
                menuChoice = int.Parse(Console.ReadLine());
                assignmentIsUpdated = true;

                // Each variable is passed as a parameter in the
                // Store Procedure to update the corresponding field
                switch (menuChoice)
                {
                    case 1:
                        Console.Write("Title: ");
                        title = Console.ReadLine();
                        cmdUpdate.Parameters.Add(new SqlParameter("@Title", title));
                        break;

                    case 2:
                        Console.Write("Description: ");
                        description = Console.ReadLine();
                        cmdUpdate.Parameters.Add(new SqlParameter("@Description", description));
                        break;

                    case 3:
                        Console.Write("Submission Date and Time: ");
                        submissionDate = DateTime.Parse(Console.ReadLine());
                        cmdUpdate.Parameters.Add(new SqlParameter("@SubmissionDate", submissionDate));
                        break;

                    case 4:
                        Console.Write("Oral Mark: ");
                        oralMark = decimal.Parse(Console.ReadLine());
                        cmdUpdate.Parameters.Add(new SqlParameter("@OralMark", oralMark));
                        break;

                    case 5:
                        Console.Write("Total Mark: ");
                        totalMark = decimal.Parse(Console.ReadLine());
                        cmdUpdate.Parameters.Add(new SqlParameter("@TotalMark", totalMark));
                        break;

                    case 6:
                        // Call of method to check if the FK is reference the PK table Course
                        courseId = db.CheckForeignKey(courseId, "Assignment");
                        cmdUpdate.Parameters.Add(new SqlParameter("@CourseId", courseId));
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        assignmentIsUpdated = false;
                        break;
                }
            }

            // Store Procedure parameters
            cmdUpdate.Parameters.Add(new SqlParameter("@TitleAsParam", titleAsSpParam));
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


        // Delete an assignment from the database
        public static string DeleteAssignment()
        {
            // Local variables declaration
            int assignmentId = 0;
            string title = string.Empty;
            bool selectionIsValid; // Control flow

            // The available parameters to delete an assignment
            do
            {
                // Insert assignment data from the console
                Console.Clear();
                Console.WriteLine("\n- Assignment Data Deletion\n");
                Console.WriteLine("Select how you want to search for an Assignment\n");
                Console.WriteLine("1. Search by ID");
                Console.WriteLine("2. Search by Title");

                Console.Write("\nEnter your preference: ");
                int menuChoice = int.Parse(Console.ReadLine());
                selectionIsValid = true;

                switch (menuChoice)
                {
                    case 1:
                        Console.Write("ID: ");
                        assignmentId = int.Parse(Console.ReadLine());
                        break;

                    case 2:
                        Console.Write("Title: ");
                        title = Console.ReadLine();
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

            // Create the SQL command to delete an assignment
            // Define the type of command as a Store Procedure
            SqlCommand cmdDelete = new SqlCommand("spAssignmentCRUD", db.SqlConnection);
            cmdDelete.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdDelete.Parameters.Add(new SqlParameter("@Id", assignmentId));
            cmdDelete.Parameters.Add(new SqlParameter("@Title", title));
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
                .Append($"Title: {Title}" + "|")
                .Append($"Description: {Description}" + "|")
                .Append($"Submission Date and Time: {SubmissionDate}" + "|")
                .Append($"Oral Mark: {OralMark}" + "|")
                .Append($"Total Mark: {TotalMark}" + "|")
                .Append($"Course ID: {CourseID}");
            return sb.ToString();
        }

    }
}
