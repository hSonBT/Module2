namespace BaseProject.Core.Exceptions;

public class ValidationException: System.Exception
{
    public ValidationException(string message): base(message)
    {
        
    }

    public ValidationException(string message, Dictionary<string, string[]> errors) : base(message)
    {
        Error = errors;
    }
    
    public Dictionary<string, string[]>? Error { get; }
}