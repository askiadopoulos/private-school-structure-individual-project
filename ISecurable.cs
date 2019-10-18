using System;

namespace IndividualProject
{
    interface ISecurable
    {
        // Interface Members
        string GetRandomSalt();
        string HashPassword(string submittedPassword);
        string HashEnhancedPassword(string submittedEnhancedPassword);
        bool VerifyPassword(string submittedPassword, string hashedPassword);
        bool VerifyEnhancedPassword(string submittedPassword, string enhancedHashedPassword);
        string MaskPassword(string password);
    }

}
