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
    // Map the class Student to the corresponding database table
    [Table(Name = "Student")]
    class Student
    {
        // Designating properties to represent the corresponding table columns in database
        [Column(Name = "ID")]
        public int ID { get; set; }
        [Column(Name = "FName")]
        public string FirstName { get; set; }
        [Column(Name = "LName")]
        public string LastName { get; set; }
        [Column(Name = "DateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [Column(Name = "TuitionFees")]
        public decimal TuitionFees { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }


        // Create assignments and store them in the database
        public static string CreateStudent()
        {
            // Insert student data from the console
            Console.Clear();
            Console.WriteLine("\n- Students Data Creation\n");
            Console.Write("FirstName: ");
            string firstname = Console.ReadLine();
            Console.Write("LastName: ");
            string lastname = Console.ReadLine();
            Console.Write("Date of Birth (dd-mm-yyyy): ");
            var dateOfBirth = DateTime.Parse(Console.ReadLine());
            Console.Write("Tuition Fees: ");
            decimal tuitionFees = decimal.Parse(Console.ReadLine());

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert a student
            // Define the type of command as a Store Procedure
            SqlCommand cmdInsert = new SqlCommand("spStudentCRUD", db.SqlConnection);
            cmdInsert.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdInsert.Parameters.Add(new SqlParameter("@FName", firstname));
            cmdInsert.Parameters.Add(new SqlParameter("@LName", lastname));
            cmdInsert.Parameters.Add(new SqlParameter("@DateOfBirth", dateOfBirth));
            cmdInsert.Parameters.Add(new SqlParameter("@TuitionFees", tuitionFees));
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


        // Select all the students from the database
        public static string ReadStudent()
        {
            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert a student
            // Define the type of command as a Store Procedure
            SqlCommand cmdSelect = new SqlCommand("spStudentCRUD", db.SqlConnection);
            cmdSelect.CommandType = CommandType.StoredProcedure;
            cmdSelect.Parameters.Add(new SqlParameter("@StatementType", "SELECT"));

            // Create a reader to retrieve the rows with all the students from the database
            SqlDataReader readerStudents = cmdSelect.ExecuteReader();
            // A list to store the retrieved rows of all the Students
            List<Student> students = new List<Student>();

            // While there are rows with students, the reader reads
            while (readerStudents.Read())
            {
                // Instantiate a new student and initialize its properties
                // with the appropriate values from the database
                Student student = new Student
                {
                    ID = readerStudents.GetInt32(0),
                    FirstName = readerStudents.GetString(1),
                    LastName = readerStudents.GetString(2),
                    DateOfBirth = readerStudents.GetDateTime(3),
                    TuitionFees = readerStudents.GetDecimal(4)
                };
                // Adds the new student in the list with students
                students.Add(student);
            }

            // Iterate the list with students and print their data
            Console.Clear();
            Console.WriteLine("\n- Students Data Retrieval\n");

            foreach (Student student in students)
            {
                Console.WriteLine(student.ToString());
            }
            string message = "\nPress any key to continue...";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Update student data in database table
        public static string UpdateStudent()
        {
            // Local variables declaration
            int studentId = 0, menuChoice = 0; // PK
            string firstname = string.Empty, lastname = string.Empty;
            DateTime dateOfBirth = DateTime.Now;
            decimal tuitionFees = 0;
            string fullnameAsSpParam = string.Empty; // Passed as a parameter in the Store Procedure
            bool choiceIsValid = true, studentIsFound = false, studentIsUpdated = false; // Control flow

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Establish a connection between code-based data structures and the
            // database itself to retrieve objects (from db) and submit changes
            DataContext dataContext = new DataContext(db.SqlConnection);
            // Act as the logical, typed table for the queries against table Student in the database
            Table<Student> students = dataContext.GetTable<Student>();

            // Create the SQL command to update a student
            // Define the type of command as a Store Procedure
            SqlCommand cmdUpdate = new SqlCommand("spStudentCRUD", db.SqlConnection);
            cmdUpdate.CommandType = CommandType.StoredProcedure;

            // Search for a student to update data based on parameters
            do
            {
                Console.Clear();
                Console.WriteLine("\n- Students Data Update\n");
                Console.WriteLine("Select how you want to search for a Student\n");
                Console.WriteLine("1. Search by ID");
                Console.WriteLine("2. Search by Fullname");
                Console.Write("\nEnter your preference: ");
                menuChoice = int.Parse(Console.ReadLine());
                choiceIsValid = true;

                switch (menuChoice)
                {
                    case 1:
                        Console.Write("\nID: ");
                        studentId = int.Parse(Console.ReadLine());

                        // In order to search for a student based on its ID
                        // we need to pass it as a parameter in the Store Procedure
                        cmdUpdate.Parameters.Add(new SqlParameter("@Id", studentId));

                        // LINQ Lambda expression:
                        // Filter all the ids from table Course and get the one requested
                        var id = students.Where(s => s.ID.Equals(studentId));
                        // If id exists in table Student
                        studentIsFound = id.Any() ? true : false;
                        break;

                    case 2:
                        Console.Write("\nFirstName: ");
                        firstname = Console.ReadLine();
                        Console.Write("LastName: ");
                        lastname = Console.ReadLine();

                        // LINQ Lambda expression:
                        // Filter all the firstnames and lastnames from table Student and get the ones requested
                        var queryFullname = students.Where(t => t.FirstName.Equals(firstname) && t.LastName.Equals(lastname));

                        // Concatenate the Firstname and Lastname as Fullname
                        fullnameAsSpParam = string.Concat(firstname, lastname);
                        // If fullname exists in table Student
                        studentIsFound = queryFullname.Any() ? true : false;
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        choiceIsValid = false;
                        break;
                }
            } while (!choiceIsValid);

            // Update student's data from the console
            while (choiceIsValid && studentIsFound && !studentIsUpdated)
            {
                // Get the student that was found by title
                var selectedStudent =
                students.Where(s => s.ID == studentId || s.FirstName == firstname && s.LastName == lastname);

                // Print all the selected student's data
                if (selectedStudent.Any())
                {
                    Console.Clear();
                    foreach (var student in selectedStudent)
                    {
                        Console.WriteLine($"\nStudent exists in database: {student.ToString()}\n");
                    }
                }

                Console.WriteLine("\nSelect data of Student to update\n");
                Console.WriteLine("1. FirstName");
                Console.WriteLine("2. LastName");
                Console.WriteLine("3. Date Of Birth");
                Console.WriteLine("4. Tuition Fees");
                Console.Write("\nEnter your preference: ");
                menuChoice = int.Parse(Console.ReadLine());
                studentIsUpdated = true;

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
                        Console.Write("Date Of Birth (dd-mm-yyyy): ");
                        dateOfBirth = DateTime.Parse(Console.ReadLine());
                        cmdUpdate.Parameters.Add(new SqlParameter("@DateOfBirth", dateOfBirth));
                        break;

                    case 4:
                        Console.Write("Tuition Fees: ");
                        tuitionFees = decimal.Parse(Console.ReadLine());
                        cmdUpdate.Parameters.Add(new SqlParameter("@TuitionFees", tuitionFees));
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        studentIsUpdated = false;
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
                + "\nPress any key to continue..." : "\nNon-existent Primary Key or Title. Update Failed."
                + "\nPress any key to return to the CRUD menu...";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Delete a student from the database
        public static string DeleteStudent()
        {
            // Local variables declaration
            int studentId = 0;
            string firstname = string.Empty, lastname = string.Empty;
            bool selectionIsValid; // Control flow

            // The available parameters to delete a student
            do
            {
                // Insert student data from the console
                Console.Clear();
                Console.WriteLine("\n- Student Data Deletion\n");
                Console.WriteLine("Select how you want to search for a Student\n");
                Console.WriteLine("1. Search by ID");
                Console.WriteLine("2. Search by FullName");

                Console.Write("\nEnter your preference: ");
                int menuChoice = int.Parse(Console.ReadLine());
                selectionIsValid = true;

                switch (menuChoice)
                {
                    case 1:
                        Console.Write("\nID: ");
                        studentId = int.Parse(Console.ReadLine());
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

            // Create the SQL command to delete a student
            // Define the type of command as a Store Procedure
            SqlCommand cmdDelete = new SqlCommand("spStudentCRUD", db.SqlConnection);
            cmdDelete.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdDelete.Parameters.Add(new SqlParameter("@Id", studentId));
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
                .Append($"Date of Birth: {DateOfBirth.ToShortDateString()}" + "|")
                .Append($"Tuition Fees: {TuitionFees}");
            return sb.ToString();
        }

    }
}
