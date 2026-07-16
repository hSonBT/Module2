namespace Lesson04_OOP;

public class Department
{
    // auto properties (nullable, not null)
    // fields + properties (get + set)

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime CreatedDate { get; set; }

    public ushort Year {private get; set; }

    public string Color { get; init; } = null;

    public string CreatedDateFormat
    {
        get {return CreatedDate.ToString("dd/MM/yyyy");}
    }
}