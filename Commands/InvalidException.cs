namespace Book.Commands;

public class InvalidException : Exception
{
    public InvalidException(string message) : base(message)
    {
    }
}