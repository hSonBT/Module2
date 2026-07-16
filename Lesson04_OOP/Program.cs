namespace Lesson04_OOP;

class Program
{
    static void Main(string[] args)
    {
// UTF 8
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Employee e1 = new Employee("Nguyen Van An", 18, 1800000);


        // Sử dụng Getter
        Console.WriteLine("Sử dụng Getter");
        Console.WriteLine(e1.GetName());
        Console.WriteLine(e1.GetAge());
        Console.WriteLine(e1.GetSalary());

        // Sử dụng Property
        Console.WriteLine("Sử dụng Property");
        Console.WriteLine(e1.Name);
        Console.WriteLine(e1.Age);
        Console.WriteLine(e1.Salary);
        
        // Điểm mạnh của Property so với Getter
        Console.WriteLine("Điểm mạnh của Property so với Getter:");
        
        // Tăng tuổi lên 1
        Console.WriteLine("Tăng tuổi lên 1 bằng Getter + Setter");
        e1.SetAge(e1.GetAge() + 1);
        Console.WriteLine(e1.GetAge());

        Console.WriteLine("Tăng tuổi lên 1 bằng Property");
        e1.Age += 1;
        Console.WriteLine(e1.Age);


        // Object Initialize
        Student s1 = new Student
        {
            // Name = "Nguyen Van An", // gọi tới Property của Student
            // Age = 18, // Gọi tới Property của Student
        };
    }
}