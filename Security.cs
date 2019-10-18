using System;

namespace IndividualProject
{
    // The above class cannot be inherited
    // It implements the IHashing Interface which delivers password hashing and masking
    sealed class Security : ISecurable
    {
        // The size of random data appended to the hash string
        private const int SaltByteSize = 24; // 24 = 192 bits
        // The size of hash string
        private const int HashByteSize = 24;
        // Increase the time required to complete one hash
        private const int WorkFactor = 12;
        // The number of iterations of a hash such as SHA
        private const int HasingIterationsCount = 10101;


        // Generate random data which are appended to the hash string
        public string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(SaltByteSize);
        }


        // Hash the submitted password
        public string HashPassword(string submittedPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(submittedPassword, GetRandomSalt());
        }


        // Hash Enhanced Password (Entropy, SHA)
        public string HashEnhancedPassword(string submittedEnhancedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(submittedEnhancedPassword, BCrypt.Net.HashType.SHA384, WorkFactor);
        }


        // Validation between submitted and hashed passwords
        public bool VerifyPassword(string submittedPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(submittedPassword, hashedPassword);
        }


        // Validation between submitted and enhanced hashed passwords
        public bool VerifyEnhancedPassword(string submittedPassword, string enhancedHashedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(submittedPassword, enhancedHashedPassword, BCrypt.Net.HashType.SHA384);
        }


        // Method to mask the user password
        public string MaskPassword(string password)
        {
            //string password = string.Empty;

            // Call ReadKey method and store the result in a special local variable
            // that can decribe the character represented by the console key pressed
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            // Test that result for the Enter key.
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                // In case the Backspace key is not pressed
                if (keyInfo.Key != ConsoleKey.Backspace)
                {
                    // Replace the origin characters with the symbol (*)
                    Console.Write("*");
                    // Call the KeyChar property to store the character pressed from the keyboard
                    password += keyInfo.KeyChar;
                }
                // In case the Backspace key is pressed
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    // The password is not empty
                    if (!string.IsNullOrEmpty(password))
                    {
                        // Remove one character from the array of password characters
                        password = password.Substring(0, password.Length - 1);
                        // Get the column location of the cursor
                        int pos = Console.CursorLeft;
                        // Move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // Replace it with space
                        Console.Write(" ");
                        // Move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                keyInfo = Console.ReadKey(true);
            }
            // Add a new line because user pressed enter at the end of his/her password
            Console.WriteLine();
            return password;
        }

    }
}
