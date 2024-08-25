namespace SimpleReceiptProcessor.Exceptions;

/// <summary>
/// Exception for controller errors. Unused for now but can be used to handle invalid requests moving forward.
/// </summary>
public class InvalidRequestException : Exception
{
    public InvalidRequestException()
    {
    }

    public InvalidRequestException(string message)
        : base(message)
    {
    }

    public InvalidRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}