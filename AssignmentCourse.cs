using System;

namespace IndividualProject
{
    class AssignmentCourse
    {
        public Assignment Assignment { get; set; }
        public Course Course { get; set; }

        public static string CreateAssignmentCourse()
        {
            Console.WriteLine("Create Assingment per Course");
            Console.ReadKey();
            return null;
        }

        public static string ReadAssignmentCourse()
        {
            Console.WriteLine("Read Assingment per Course");
            Console.ReadKey();
            return null;
        }

        public static string UpdateAssignmentCourse()
        {
            Console.WriteLine("Update Assingment per Course");
            Console.ReadKey();
            return null;
        }

        public static string DeleteAssignmentCourse()
        {
            Console.WriteLine("Delete Assingment per Course");
            Console.ReadKey();
            return null;
        }

    }
}
