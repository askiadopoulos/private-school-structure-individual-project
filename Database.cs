using System;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;

namespace IndividualProject
{
    class Database
    {
        // Fields
        // The Connection string with all the necessary info for the corresponding db.
        private const string connectionString = @"Server= THINKPAD-T500\SQLEXPRESS;Database = IndividualProject;Trusted_Connection= True";

        // Properties
        // Handles the connection with the db.
        public SqlConnection SqlConnection { get; private set; }

        // Default Constructor
        public Database()
        {
            SqlConnection = new SqlConnection(connectionString);
        }


        // Checks if a Foreign Key exists in a Primary Key table
        public int CheckForeignKey(int courseId, string className)
        {
            // Establish a connection between code-based data structures and the
            // database itself to retrieve objects (from db) and submit changes
            DataContext dataContext = new DataContext(SqlConnection);

            // Act as the logical, typed table for the queries
            // against tables Course, Trainer and Assignment in the database
            Table<Course> courses = dataContext.GetTable<Course>();
            Table<Trainer> trainers = dataContext.GetTable<Trainer>();
            Table<Assignment> assignments = dataContext.GetTable<Assignment>();

            // Control if the CourseID is valid
            string courseIdExistsAsFK = "True";

            // Loop to control if the given Primary Key is valid
            do
            {
                Console.Write("Enter Course ID: ");
                courseId = int.Parse(Console.ReadLine());

                switch (className)
                {
                    case "Course":
                        // LINQ Lambda expression:
                        // Check if the given Course ID (PK) exists as a FK in the corresponding tables Trainer, Assignment
                        var trainerCourseID = trainers.Any(id => id.CourseID == courseId);
                        var assignmentCourseID = assignments.Any(id => id.CourseID == courseId);

                        Console.WriteLine(trainerCourseID && assignmentCourseID
                            ? "\nPRIMARY KEY 'ID' of table 'Course' currently exists as a FOREIGN KEY in both tables 'Trainer' and 'Assignment'.\n"
                            : trainerCourseID ? "\nPRIMARY KEY 'ID' of table 'Course' currently exists as a FOREIGN KEY in table 'Trainer'.\n"
                            : assignmentCourseID ? "\nPRIMARY KEY 'ID' of table 'Course' currently exists as a FOREIGN KEY in table 'Assignment'.\n"
                            : courseIdExistsAsFK = string.Empty);
                        break;

                    case "Trainer":
                    case "Assignment":
                        // LINQ Lambda expression:
                        // Get the highest and lowest CourseID (FK) from table Course
                        var maxCourseID = courses.Max(max => max.ID);
                        var minCourseID = courses.Min(min => min.ID);

                        // Check if the given Course ID reference the corresponding Primary Key table
                        Console.WriteLine(courseId > maxCourseID || courseId < minCourseID
                            ? "\nFOREIGN KEY 'CourseID' value does not currently exists in the PRIMARY KEY Table 'Course'.\n"
                            : courseIdExistsAsFK = string.Empty);
                        break;
                }

            } while (courseIdExistsAsFK.Equals("True"));

            return courseId;
        }

    }
}
