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
    // Enumerator Collections
    public enum Stream { Java = 1, CSharp }
    public enum Type { Full = 1, Part }

    // Map the class Course to the corresponding database table
    [Table(Name = "Course")]
    class Course
    {
        // Designating properties to represent the corresponding table columns in database
        [Column(Name = "ID")]
        public int ID { get; set; }
        [Column(Name = "Title")]
        public string Title { get; set; }
        [Column(Name = "Stream")]
        public string Stream { get; set; }
        [Column(Name = "Type")]
        public string Type { get; set; }
        [Column(Name = "StartDate")]
        public DateTime StartDate { get; set; }
        [Column(Name = "EndDate")]
        public DateTime EndDate { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }


        // Create courses and store them in the database
        public static string CreateCourse()
        {
            // Insert course data from the console
            Console.Clear();
            Console.WriteLine("\n- Courses Data Creation\n");
            Console.Write("Title: ");
            string title = Console.ReadLine();
            Console.Write("Stream (1 = Java, 2 = C#): ");
            var stream = (Stream)Enum.Parse(typeof(Stream), Console.ReadLine());
            Console.Write("Type (1 = Full-Time, 2 = Part-Time): ");
            var type = (Type)Enum.Parse(typeof(Type), Console.ReadLine());
            Console.Write("Start Date (dd-mm-yyyy) (HH-mm): ");
            var startDate = Convert.ToDateTime(Console.ReadLine());
            Console.Write("End Date (dd-mm-yyyy) (HH-mm): ");
            var endDate = Convert.ToDateTime(Console.ReadLine());

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert a course
            // Define the type of command as a Store Procedure
            SqlCommand cmdInsert = new SqlCommand("spCourseCRUD", db.SqlConnection);
            cmdInsert.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdInsert.Parameters.Add(new SqlParameter("@Title", title));
            cmdInsert.Parameters.Add(new SqlParameter("@Stream", stream));
            cmdInsert.Parameters.Add(new SqlParameter("@Type", type));
            cmdInsert.Parameters.Add(new SqlParameter("@StartDate", startDate));
            cmdInsert.Parameters.Add(new SqlParameter("@EndDate", endDate));
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


        // Select all the courses from the database
        public static string ReadCourse()
        {
            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert a course
            // Define the type of command as a Store Procedure
            SqlCommand cmdSelect = new SqlCommand("spCourseCRUD", db.SqlConnection);
            cmdSelect.CommandType = CommandType.StoredProcedure;
            cmdSelect.Parameters.Add(new SqlParameter("@StatementType", "SELECT"));

            // Create a reader to retrieve the rows with all the courses from the database
            SqlDataReader readerCourses = cmdSelect.ExecuteReader();
            // A list to store the retrieved rows of all the courses
            List<Course> courses = new List<Course>();

            // While there are rows with courses, the reader reads
            while (readerCourses.Read())
            {
                // Instantiate a new course and initialize its properties
                // with the appropriate values from the database
                Course course = new Course
                {
                    ID = readerCourses.GetInt32(0),
                    Title = readerCourses.GetString(1),
                    Stream = readerCourses.GetString(2),
                    Type = readerCourses.GetString(3),
                    StartDate = readerCourses.GetDateTime(4),
                    EndDate = readerCourses.GetDateTime(5)
                };
                // Adds the new course in the list with courses
                courses.Add(course);
            }

            // Iterate the list with courses and print their data
            Console.Clear();
            Console.WriteLine("\n- Courses Data Retrieval\n");

            foreach (Course course in courses)
            {
                Console.WriteLine(course.ToString());
            }
            string message = "\nPress any key to continue...";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Update course data in database table
        public static string UpdateCourse()
        {
            // Local variables declaration
            int courseId = 0, menuChoice = 0; // PK
            string title = string.Empty, stream = string.Empty, type = string.Empty;
            DateTime startDateTime = DateTime.Now, endDateTime = DateTime.Now;
            string titleAsSpParam = string.Empty; // Passed as a parameter in the Store Procedure
            bool choiceIsValid = true, courseIsFound = false, courseIsUpdated = false; // Control flow

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Establish a connection between code-based data structures and the
            // database itself to retrieve objects (from db) and submit changes
            DataContext dataContext = new DataContext(db.SqlConnection);
            // Act as the logical, typed table for the queries against table Course in the database
            Table<Course> courses = dataContext.GetTable<Course>();

            // Create the SQL command to update a course
            // Define the type of command as a Store Procedure
            SqlCommand cmdUpdate = new SqlCommand("spCourseCRUD", db.SqlConnection);
            cmdUpdate.CommandType = CommandType.StoredProcedure;

            // Search for a course to update data based on parameters
            do
            {
                Console.Clear();
                Console.WriteLine("\n- Courses Data Update\n");
                Console.WriteLine("Select how you want to search for a Course\n");
                Console.WriteLine("1. Search by ID");
                Console.WriteLine("2. Search by Title");
                Console.Write("\nEnter your preference: ");
                menuChoice = int.Parse(Console.ReadLine());
                choiceIsValid = true;

                switch (menuChoice)
                {
                    case 1:
                        Console.Write("\nID: ");
                        courseId = int.Parse(Console.ReadLine());

                        // In order to search for a course based on its ID
                        // we need to pass it as a parameter in the Store Procedure
                        cmdUpdate.Parameters.Add(new SqlParameter("@Id", courseId));

                        // LINQ Lambda expression:
                        // Filter all the ids from table Course and get the one requested
                        var id = courses.Where(c => c.ID.Equals(courseId));
                        // If id exists in table Course
                        courseIsFound = id.Any() ? true : false;
                        break;

                    case 2:
                        Console.Write("\nTitle: ");
                        // Pass the course title as parameter in the Store Procedure
                        titleAsSpParam = title = Console.ReadLine();

                        // LINQ Lambda expression:
                        // Filter all the titles from table Course and get the ones requested
                        var queryTitle = courses.Where(c => c.Title.Equals(title));

                        // If title exists in table Course
                        courseIsFound = queryTitle.Any() ? true : false;
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        choiceIsValid = false;
                        break;
                }
            } while (!choiceIsValid);

            // Update course's data from the console
            while (choiceIsValid && courseIsFound && !courseIsUpdated)
            {
                // Get the course that was found by title
                var selectedCourse =
                courses.Where(c => c.ID == courseId || c.Title == title);

                // Print all the selected course's data
                if (selectedCourse.Any())
                {
                    Console.Clear();
                    foreach (var course in selectedCourse)
                    {
                        Console.WriteLine($"\nCourse exists in database: {course.ToString()}\n");
                    }
                }

                Console.WriteLine("\nSelect data of Course to update\n");
                Console.WriteLine("1. Title");
                Console.WriteLine("2. Stream");
                Console.WriteLine("3. Type");
                Console.WriteLine("4. Start Date and Time");
                Console.WriteLine("5. End Date and Time");
                Console.Write("\nEnter your preference: ");
                menuChoice = int.Parse(Console.ReadLine());
                courseIsUpdated = true;

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
                        Console.Write("Stream: ");
                        stream = Console.ReadLine();
                        cmdUpdate.Parameters.Add(new SqlParameter("@Stream", stream));
                        break;

                    case 3:
                        Console.Write("Type: ");
                        type = Console.ReadLine();
                        cmdUpdate.Parameters.Add(new SqlParameter("@Type", type));
                        break;

                    case 4:
                        Console.Write("Start Date and Time (dd-mm-yyyy) (HH-mm): ");
                        startDateTime = DateTime.Parse(Console.ReadLine());
                        cmdUpdate.Parameters.Add(new SqlParameter("@StartDate", startDateTime));
                        break;

                    case 5:
                        Console.Write("End Date and Time: (dd-mm-yyyy) (HH-mm)");
                        endDateTime = DateTime.Parse(Console.ReadLine());
                        cmdUpdate.Parameters.Add(new SqlParameter("@EndDate", endDateTime));
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        courseIsUpdated = false;
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
                + "\nPress any key to continue..." : "\nNon-existent Primary Key or Title. Update Failed."
                + "\nPress any key to return to the CRUD menu...";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Delete a course from the database
        public static string DeleteCourse()
        {
            // Local variables declaration
            int courseId = 0;

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Insert course data from the console
            Console.Clear();
            Console.WriteLine("\n- Course Data Deletion\n");

            // Call of method to check if the ID (PK) is reference other tables as a FK
            courseId = db.CheckForeignKey(courseId, "Course");

            // Create the SQL command to delete a trainer
            // Define the type of command as a Store Procedure
            SqlCommand cmdDelete = new SqlCommand("spCourseCRUD", db.SqlConnection);
            cmdDelete.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdDelete.Parameters.Add(new SqlParameter("@Id", courseId));
            cmdDelete.Parameters.Add(new SqlParameter("@StatementType", "DELETE"));

            // Check the number of rows affected
            int deletedRows = cmdDelete.ExecuteNonQuery();
            // And print the appropriate message
            string message = deletedRows > 0 ? "\nDeletion Success. " + $"{deletedRows} Row(s) erased successfully."
                + "\nPress any key to continue..." : "\nNon-existent Primary Key or Title. Deletion Failed."
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
                .Append($"Stream: {Stream}" + "|")
                .Append($"Type: {Type}" + "|")
                .Append($"Start Date: {StartDate}" + "|")
                .Append($"End Date: {EndDate}");
            return sb.ToString();
        }

    }
}
