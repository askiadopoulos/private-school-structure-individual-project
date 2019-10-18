using System;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;

namespace IndividualProject
{
    // Stores the different permission profiles (roles) for the users
    public enum Role
    {
        HeadMaster = 1,
        Trainer,
        Student,
        Unauthorized = -1
    }

    // Map the class User to the corresponding database table
    [Table(Name = "Users")]
    class User
    {
        // One User consists of one integer (ID: AI) and two strings (Username: Unique, Password: Hash)
        // Designating properties to represent the corresponding table columns in database
        [Column(Name = "ID")]
        private int ID { get; set; }
        [Column(Name = "Username")]
        private string Username { get; set; }
        [Column(Name = "Password")]
        private string Password { get; set; }
        [Column(Name = "RoleID")]
        public int RoleID { get; set; }


        // Create and store a new user in the database
        private static string CreateUser()
        {
            // Insert user data from the console
            Console.Clear();
            Console.WriteLine("\n- User Data Creation\n");
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");

            // Create a reference to call method for password hashing
            var security = new Security();
            // Call method to hash the given password before storing it in the database
            string passwordHash = security.HashEnhancedPassword(Console.ReadLine());

            Console.WriteLine("Role ID prefix values: 1 = Head Master, 2 = Trainer, 3 = Student, 4 = Unauthorized");
            Console.Write("Enter Role ID (1-4): ");
            int roleId = (int)Enum.Parse(typeof(Role), Console.ReadLine());

            // Parse the Role ID in database as -1 (unathorized users)
            if (roleId == 4) { roleId = -1; }

            // Create an object to connect with the database
            Database db = new Database();
            db.SqlConnection.Open();

            // Create the SQL command to insert a user
            // Define the type of command as a Store Procedure
            SqlCommand cmdInsert = new SqlCommand("spUsersCRUD", db.SqlConnection);
            cmdInsert.CommandType = CommandType.StoredProcedure;

            // Store Procedure parameters
            cmdInsert.Parameters.Add(new SqlParameter("@Username", username));
            cmdInsert.Parameters.Add(new SqlParameter("@Password", passwordHash));
            cmdInsert.Parameters.Add(new SqlParameter("@RoleId", roleId));

            // TODO: Check constraints concerning the reference FK in table Role
            // Store Procedure parameters: The Role ID can now be commited
            //cmdInsert.Parameters.Add(new SqlParameter("@RoleId", roleId));
            cmdInsert.Parameters.Add(new SqlParameter("@StatementType", "INSERT"));

            // Check the number of rows affected
            int insertedRows = cmdInsert.ExecuteNonQuery();
            // And print the appropriate message
            string message = insertedRows > 0 ? "\nInsert Success. " + $"{insertedRows} Row(s) commited successfully.\n"
                : "\nInsert Failed.\n";

            db.SqlConnection.Close(); // Close connection with the database
            db.SqlConnection.Dispose(); // Reset the state of the SqlConnection object

            return message;
        }


        // Handles the user authedication and login task.
        public int AuthedicateUser()
        {
            // The object contains an SqlConnection property which includes the connection string.
            Database db = new Database();
            // Instantiate class Security to mask and verify the submitted password
            Security security = new Security();

            bool userExists = true;
            int userRoleID = 0;

            // Establish a connection between code-based data structures and the
            // database itself to retrieve objects (from db) and submit changes
            DataContext dataContext = new DataContext(db.SqlConnection);
            // Act as the logical, typed table for the queries against table Users in the database
            Table<User> users = dataContext.GetTable<User>();

            // Using block: acquires the system resources that the block controls.
            // In otherwords, sqlConnection variable will be disposed upon completion.
            using (db.SqlConnection)
            {
                try
                {
                    // Open a connection with the db.
                    db.SqlConnection.Open();
                    do
                    {
                        // Insert Username and Password from the console
                        Console.Write("\n\n- Enter your credentials to login:");
                        Console.Write("\n- Username: ");
                        string username = Console.ReadLine();
                        Console.Write("- Password: ");

                        // Masking user's submitted password
                        string plainPassword = security.MaskPassword(string.Empty);

                        SqlCommand cmdLogin = new SqlCommand("spUsersLogin", db.SqlConnection);
                        cmdLogin.CommandType = CommandType.StoredProcedure;

                        // LINQ Lambda expression:
                        // Filter all the usernames from table Users, get the one requested
                        var queryUsername = users.DefaultIfEmpty().Single(u => u.Username == username);

                        int? result;
                        result = string.Compare(queryUsername.Username, username);
                        bool doesPasswordMatch = false;

                        // If submitted username mathes the one stored in database
                        if (result == 0)
                        {
                            // Assign the corresponding password to a string variable
                            string hashedPassword = queryUsername.Password;

                            // Verify user's password
                            // Compare the submitted password with the one stored in database (hashed)
                            doesPasswordMatch = security.VerifyEnhancedPassword(plainPassword, hashedPassword);

                            if (doesPasswordMatch)
                            {
                                // Store Procedure parameters
                                cmdLogin.Parameters.Add(new SqlParameter("@Username", username));
                                cmdLogin.Parameters.Add(new SqlParameter("@Password", hashedPassword));
                            }
                        }

                        // Call a method that returns one object
                        SqlDataReader reader = cmdLogin.ExecuteReader();

                        // Check if an SqlDataReader object (reader) has a row/rows or not.
                        if (!reader.HasRows && !doesPasswordMatch)
                        {
                            userExists = false;
                            Console.Write("\nUsername and/or Password Invalid. Press any key to try again...");
                            Console.ReadKey();
                        }

                        // Reading from the data stream (reader) by pulling data
                        // Check user's permissions to login into the system
                        while (reader.Read())
                        {
                            // Get the values of the specified columns
                            userRoleID = reader.GetInt32(0);

                            switch (userRoleID)
                            {
                                // User has no authorization
                                case -1:
                                    Console.WriteLine("\nYou are not authorized to access the system.");
                                    Console.Write("Press any key to try again...");
                                    Console.ReadKey();
                                    userExists = false;
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    Console.WriteLine("\nLogged in successfully. You do not have administrator rights to make changes.");
                                    Console.Write("Press any key to continue...");
                                    Console.ReadKey();
                                    break;
                                case 10:
                                    Console.WriteLine("\nLogged in successfully. You have permissions to make changes.");
                                    Console.Write("Press any key to continue...");
                                    Console.ReadKey();
                                    Console.WriteLine(CreateUser());
                                    break;
                            }
                        }
                        reader.Close();

                    } while (!userExists);
                }
                catch (Exception)
                {
                    Console.Write("\nThere is no such Username and/or Password !.\n");
                    Console.ReadKey();
                }
                finally
                {
                    db.SqlConnection.Close();
                }
            }
            return userRoleID;
        }

    }
}
