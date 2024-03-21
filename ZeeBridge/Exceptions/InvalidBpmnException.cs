namespace ZeeBridge.Exceptions;

public class InvalidBpmnException : Exception
{
    public InvalidBpmnException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}