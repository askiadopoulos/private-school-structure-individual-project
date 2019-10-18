using System;

namespace IndividualProject
{
    // The Head Master options menu available choices
    public enum HeadMasterMenu
    {
        CrudCourses = 1,
        CrudStudents,
        CrudAssignments,
        CrudTrainers,
        CrudStudentsCourses,
        CrudTrainersCourses,
        CrudAssignmentsCourses,
        CrudScheduleCourses,
        Quit
    }
    // The Trainer options menu available choices
    public enum TrainerMenu
    {
        ViewEnrolledCourses = 1,
        ViewStudentsCourse,
        ViewAssignentsStudentCourse,
        MarkAssignmentsStudentCourse,
        Quit
    }
    // The Student options menu available choices
    public enum StudentMenu
    {
        ViewDailyScheduleCourse = 1,
        ViewDatesOfSubmissionAssignmentsCourse,
        SubmitAssignments,
        Quit
    }
    // The CRUD options menu available operations (refers to the Head Master)
    public enum CrudMenu
    {
        Create = 1,
        Read,
        Update,
        Delete,
        Return,
        Quit
    }
    // The CRUD options menu available entities (refers to the Head Master)
    public enum CrudOperationMenu
    {
        Course = 1,
        Student,
        Assignment,
        Trainer,
        StudentsCourses,
        TrainersCourses,
        AssignmentsCourses,
        ScheduleCourses
    }

    // Class UI is responsible for the user interface options menus in the console
    class UserInterface
    {
        // The Main Options Menu
        public void DisplayMainMenu(int userRoleId)
        {
            switch (userRoleId)
            {
                case 1:
                    DisplayHeadMasterMenu();
                    break;
                case 2:
                    DisplayTrainerMenu();
                    break;
                case 3:
                    DisplayStudentMenu();
                    break;
            }
        }


        // Method implements the User Menu
        public int LoginConsole()
        {
            // Local variables declaration
            bool executeMenuAgain = true;
            //int userRoleID = 0;

            // The object contains an SqlConnection property which includes the connection string.
            Database db = new Database();
            User user = new User();

            do
            {
                executeMenuAgain = true;

                // Users Menu Options
                Console.Clear();
                Console.WriteLine("\n------------------------------");
                Console.WriteLine("// Private School Structure //");
                Console.WriteLine("------------------------------\n");
                Console.WriteLine("1. Login as an existing user");
                Console.WriteLine("2. Register a new user (only as administrator)\n");
                Console.Write("-- Press the corresponding key or ESC to quit...");

                // Grab the key pressed by the user
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Terminate processes and quit program
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    db.SqlConnection.Close(); db.SqlConnection.Dispose();
                    Console.WriteLine(); Environment.Exit(0);
                }
                // Access the Login Console
                else if (keyInfo.Key == ConsoleKey.D1 || keyInfo.Key == ConsoleKey.D2)
                {
                    user.RoleID = user.AuthedicateUser();
                    executeMenuAgain = false;
                }
                else
                {
                    Console.Write("\n\nWrong choice. Press any key to try again...");
                    Console.ReadKey();
                }

            } while (executeMenuAgain);

            return user.RoleID;
        }


        // The Head Master Options Menu
        private void DisplayHeadMasterMenu(/*int userRoleId*/)
        {
            // Controls the execution of the Head Master options menu
            bool isHeadMasterMenuActive;
            short headMasterChoice = 0;

            // Loop to navigate through the Head Master options menu
            do
            {
                // Display Head Master options menu labels
                Console.Clear();
                Console.WriteLine("\n------------------------------");
                Console.WriteLine("// Private School Structure //");
                Console.WriteLine("------------------------------\n");
                Console.WriteLine("- Head Master Options Menu\n");
                Console.WriteLine("1. CRUD on Courses");
                Console.WriteLine("2. CRUD on Students");
                Console.WriteLine("3. CRUD on Assignments");
                Console.WriteLine("4. CRUD on Trainers");
                Console.WriteLine("5. CRUD on Students per Courses");
                Console.WriteLine("6. CRUD on Trainers per Courses");
                Console.WriteLine("7. CRUD on Assignments per Courses");
                Console.WriteLine("8. CRUD on Schedule per Courses");
                Console.WriteLine("9. Quit program");
                Console.Write("\nEnter your preference: ");

                // Head Master input his/her choice
                headMasterChoice = short.Parse(Console.ReadLine());
                isHeadMasterMenuActive = true;

                // Head Master options
                switch (headMasterChoice)
                {
                    case (short)HeadMasterMenu.CrudCourses:
                        DisplayCrudMenu((short)HeadMasterMenu.CrudCourses);
                        break;

                    case (short)HeadMasterMenu.CrudStudents:
                        DisplayCrudMenu((short)HeadMasterMenu.CrudStudents);
                        break;

                    case (short)HeadMasterMenu.CrudAssignments:
                        DisplayCrudMenu((short)HeadMasterMenu.CrudAssignments);
                        break;

                    case (short)HeadMasterMenu.CrudTrainers:
                        DisplayCrudMenu((short)HeadMasterMenu.CrudTrainers);
                        break;

                    case (short)HeadMasterMenu.CrudStudentsCourses:
                        DisplayCrudMenu((short)HeadMasterMenu.CrudStudentsCourses);
                        break;

                    case (short)HeadMasterMenu.CrudTrainersCourses:
                        DisplayCrudMenu((short)HeadMasterMenu.CrudTrainersCourses);
                        break;

                    case (short)HeadMasterMenu.CrudAssignmentsCourses:
                        DisplayCrudMenu((short)HeadMasterMenu.CrudAssignmentsCourses);
                        break;

                    case (short)HeadMasterMenu.CrudScheduleCourses:
                        DisplayCrudMenu((short)HeadMasterMenu.CrudScheduleCourses);
                        break;

                    case (short)HeadMasterMenu.Quit:
                        Console.WriteLine("\nWaiting for database connections to close...");
                        System.Threading.Thread.Sleep(1000);
                        isHeadMasterMenuActive = false;
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }

            } while (isHeadMasterMenuActive);
        }


        // The CRUD Options Menu of the Head Master
        private void DisplayCrudMenu(short crudOperation)
        {
            // Controls the execution of the CRUD options menu
            bool isCrudMenuActive;
            // Passed as parameter (interpolation) inside the CRUD options menu
            string crudLabel = string.Empty;

            // Nested Ternary Operator to handle the CRUD options menu labels
            crudLabel = crudOperation.Equals(1) ? crudLabel = "Courses"
                : crudOperation.Equals(2) ? crudLabel = "Students"
                : crudOperation.Equals(3) ? crudLabel = "Assignments"
                : crudOperation.Equals(4) ? crudLabel = "Trainers"
                : crudOperation.Equals(5) ? crudLabel = "Students per Courses"
                : crudOperation.Equals(6) ? crudLabel = "Trainers per Courses"
                : crudOperation.Equals(7) ? crudLabel = "Assignments per Courses"
                : crudLabel = "Schedule per Courses";

            // Loop to navigate through the CRUD options menu
            do
            {
                // Display CRUD options menu labels
                Console.Clear();
                Console.WriteLine("\n------------------------------");
                Console.WriteLine("// Private School Structure //");
                Console.WriteLine("------------------------------\n");
                Console.WriteLine("- CRUD Options Menu\n");
                Console.WriteLine($"1. Create {crudLabel}");
                Console.WriteLine($"2. Read {crudLabel}");
                Console.WriteLine($"3. Update {crudLabel}");
                Console.WriteLine($"4. Delete {crudLabel}");
                Console.WriteLine("5. Return to the Head Master menu");
                Console.WriteLine("6. Quit Program");
                Console.Write("\nEnter your preference: ");

                // Head Master input his/her choice
                short crudChoice = short.Parse(Console.ReadLine());
                isCrudMenuActive = true;
                string option = string.Empty;

                // CRUD options
                switch (crudChoice)
                {
                    case (short)CrudMenu.Create:
                        option = crudOperation.Equals((short)CrudOperationMenu.Course) ? Course.CreateCourse()
                            : crudOperation.Equals((short)CrudOperationMenu.Student) ? Student.CreateStudent()
                            : crudOperation.Equals((short)CrudOperationMenu.Assignment) ? Assignment.CreateAssignment()
                            : crudOperation.Equals((short)CrudOperationMenu.Trainer) ? Trainer.CreateTrainer()
                            : crudOperation.Equals((short)CrudOperationMenu.StudentsCourses) ? StudentCourse.CreateStudentCourse()
                            // Create TrainerCourses + ScheduleCourses
                            : crudOperation.Equals((short)CrudOperationMenu.AssignmentsCourses) ? AssignmentCourse.CreateAssignmentCourse()
                            : option;
                        break;

                    case (short)CrudMenu.Read:
                        option = crudOperation.Equals((short)CrudOperationMenu.Course) ? Course.ReadCourse()
                            : crudOperation.Equals((short)CrudOperationMenu.Student) ? Student.ReadStudent()
                            : crudOperation.Equals((short)CrudOperationMenu.Assignment) ? Assignment.ReadAssignment()
                            : crudOperation.Equals((short)CrudOperationMenu.Trainer) ? Trainer.ReadTrainer()
                            : crudOperation.Equals((short)CrudOperationMenu.StudentsCourses) ? StudentCourse.ReadStudentCourse()
                            // Read TrainerCourses + ScheduleCourses
                            : crudOperation.Equals((short)CrudOperationMenu.AssignmentsCourses) ? AssignmentCourse.ReadAssignmentCourse()
                            : option;
                        break;

                    case (short)CrudMenu.Update:
                        option = crudOperation.Equals((short)CrudOperationMenu.Course) ? Course.UpdateCourse()
                            : crudOperation.Equals((short)CrudOperationMenu.Student) ? Student.UpdateStudent()
                            : crudOperation.Equals((short)CrudOperationMenu.Assignment) ? Assignment.UpdateAssignment()
                            : crudOperation.Equals((short)CrudOperationMenu.Trainer) ? Trainer.UpdateTrainer()
                            : crudOperation.Equals((short)CrudOperationMenu.StudentsCourses) ? StudentCourse.UpdateStudentCourse()
                            // Update TrainerCourses + ScheduleCourses
                            : crudOperation.Equals((short)CrudOperationMenu.AssignmentsCourses) ? AssignmentCourse.UpdateAssignmentCourse()
                            : option;
                        break;

                    case (short)CrudMenu.Delete:
                        option = crudOperation.Equals((short)CrudOperationMenu.Course) ? Course.DeleteCourse()
                            : crudOperation.Equals((short)CrudOperationMenu.Student) ? Student.DeleteStudent()
                            : crudOperation.Equals((short)CrudOperationMenu.Assignment) ? Assignment.DeleteAssignment()
                            : crudOperation.Equals((short)CrudOperationMenu.Trainer) ? Trainer.DeleteTrainer()
                            : crudOperation.Equals((short)CrudOperationMenu.StudentsCourses) ? StudentCourse.DeleteStudentCourse()
                            // Delete TrainerCourses + ScheduleCourses
                            : crudOperation.Equals((short)CrudOperationMenu.AssignmentsCourses) ? AssignmentCourse.DeleteAssignmentCourse()
                            : option;
                        break;

                    case (short)CrudMenu.Return:
                        return;

                    case (short)CrudMenu.Quit:
                        Console.WriteLine("\nWaiting for database connections to close...");
                        System.Threading.Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        break;
                }
                Console.Write(option);
                Console.ReadKey();
            } while (isCrudMenuActive);
        }


        // The Trainer Options Menu
        private void DisplayTrainerMenu()
        {
            // Controls the execution of the Trainer options menu
            bool isTrainerMenuActive;

            // Loop to navigate through the Trainer options menu
            do
            {
                // Display Trainer options menu labels
                Console.Clear();
                Console.WriteLine("\n------------------------------");
                Console.WriteLine("// Private School Structure //");
                Console.WriteLine("------------------------------\n");
                Console.WriteLine("- Trainer Options Menu\n");
                Console.WriteLine("1. View all the enrolled Courses");
                Console.WriteLine("2. View all the Students per Course");
                Console.WriteLine("3. View all the Assignments per Student per Course");
                Console.WriteLine("4. Mark all the Assignments per Student per Course");
                Console.WriteLine("5. Quit Program");
                Console.Write("\nEnter your preference: ");

                // Trainer input his/her choice
                short trainerChoice = short.Parse(Console.ReadLine());
                isTrainerMenuActive = true;

                // Trainer options
                switch (trainerChoice)
                {
                    case (short)TrainerMenu.ViewEnrolledCourses:
                        // TODO: Trainer can view all the courses he/she is enrolled
                        Console.Write("Trainer can view all the courses he/she is enrolled");
                        Console.ReadKey();
                        break;

                    case (short)TrainerMenu.ViewStudentsCourse:
                        // TODO: Trainer can view all the Students per Course
                        Console.Write("Trainer can view all the Students per Course");
                        Console.ReadKey();
                        break;

                    case (short)TrainerMenu.ViewAssignentsStudentCourse:
                        // TODO: Trainer can view all the Assignents per Student per Course
                        Console.Write("Trainer can view all the Assignents per Student per Course");
                        Console.ReadKey();
                        break;

                    case (short)TrainerMenu.MarkAssignmentsStudentCourse:
                        // TODO: Trainer can mark all the Assignents per Student per Course
                        Console.Write("Trainer can mark all the Assignents per Student per Course");
                        Console.ReadKey();
                        break;

                    case (short)TrainerMenu.Quit:
                        Console.WriteLine("\nWaiting for database connections to close...");
                        System.Threading.Thread.Sleep(1000);
                        isTrainerMenuActive = false;
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }

            } while (isTrainerMenuActive);
        }


        // The Student Options Menu
        private void DisplayStudentMenu()
        {
            // Controls the execution of the Student options menu
            bool isStudentMenuActive;

            // Loop to navigate through the Student options menu
            do
            {
                // Display Student options menu labels
                Console.Clear();
                Console.WriteLine("\n------------------------------");
                Console.WriteLine("// Private School Structure //");
                Console.WriteLine("------------------------------\n");
                Console.WriteLine("- Student Options Menu\n");
                Console.WriteLine("1. View daily Schedule per Course");
                Console.WriteLine("2. View dates of submission of the Assignments per Course");
                Console.WriteLine("3. Submit any Assignments");
                Console.WriteLine("4. Quit Program");
                Console.Write("\nEnter your preference: ");

                // Student input his/her choice
                short studentChoice = short.Parse(Console.ReadLine());
                isStudentMenuActive = true;

                // Student options
                switch (studentChoice)
                {
                    case (short)StudentMenu.ViewDailyScheduleCourse:
                        // TODO: Student can view his/her daily Schedule per Course
                        Console.Write("Student can view his/her daily Schedule per Course");
                        Console.ReadKey();
                        break;

                    case (short)StudentMenu.ViewDatesOfSubmissionAssignmentsCourse:
                        // TODO: Student can view the dates of submission of the Assignments per Course
                        Console.Write("Student can view the dates of submission of the Assignments per Course");
                        Console.ReadKey();
                        break;

                    case (short)StudentMenu.SubmitAssignments:
                        // TODO: Student can submit any Assignments
                        Console.Write("Student can submit any Assignments");
                        Console.ReadKey();
                        break;

                    case (short)StudentMenu.Quit:
                        Console.WriteLine("\nWaiting for database connections to close...");
                        System.Threading.Thread.Sleep(1000);
                        isStudentMenuActive = false;
                        break;

                    default:
                        Console.Write("Wrong input. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }

            } while (isStudentMenuActive);
        }

    }
}
