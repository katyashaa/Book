namespace Book.Commands;

public class DuplicateBookException : Exception
{
    public DuplicateBookException(string message) : base(message)
    {
    }
}