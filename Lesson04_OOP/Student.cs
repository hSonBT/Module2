namespace Lesson04_OOP;

public class Student
{
    private string name = string.Empty;
    private uint age;

    // public Student()
    // {
    //     
    // }
    //
    // public Student(string name, uint age)
    // {
    //     this.name = name;
    //     this.age = age;
    // }

    public string Name
    {
        get { return this.name; }
        set { this.name = value; }
    }
    
    public uint Age
    {
        get { return this.age; }
        set { this.age = value; }
    }
    
    
}