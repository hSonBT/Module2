namespace Lesson04_OOP;

public class Employee
{
    private string name;
    private uint age;
    private uint salary;

    // Constructor
    public Employee(string name, uint age, uint salary)
    {
        this.name = name;
        this.age = age;
        this.salary = salary;
    }

    // Getters
    public string GetName()
    {
        return this.name;
    }

    public uint GetAge()
    {
        return this.age;
    }

    public uint GetSalary()
    {
        return this.salary;
    }
    
    // Setter
    public void SetName(string value)
    {
        this.name = value;
    }

    public void SetAge(uint value)
    {
        this.age = value;
    }

    public void SetSalary(uint value)
    {
        this.salary = value;
    }

    // Properties
    public string Name
    {
        // Get: mặc định là public
        get { return this.name; }
        // Set: mặc định là public
        set { this.name = value; }
    }

    public uint Age
    {
        // Get
        get { return this.age; }
        // Set
        set { this.age = value; }
    }

    public uint Salary
    {
        get { return this.salary; }
        set { this.salary = value; }
    }
    
}