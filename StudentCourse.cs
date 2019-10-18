using System;

namespace IndividualProject
{
    class StudentCourse
    {
        public Student Student { get; set; }
        public Course Course { get; set; }


        public static string CreateStudentCourse()
        {
            Console.WriteLine("Create Student per Course");
            Console.ReadKey();
            return null;
        }

        public static string ReadStudentCourse()
        {
            Console.WriteLine("Read Student per Course");
            Console.ReadKey();
            return null;
        }

        public static string UpdateStudentCourse()
        {
            Console.WriteLine("Update Student per Course");
            Console.ReadKey();
            return null;
        }

        public static string DeleteStudentCourse()
        {
            Console.WriteLine("Delete Student per Course");
            Console.ReadKey();
            return null;
        }

    }
}
