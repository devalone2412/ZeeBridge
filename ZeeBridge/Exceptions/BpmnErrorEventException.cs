namespace ZeeBridge.Exceptions;

public class BpmnErrorEventException : Exception
{
    public string Code { get; }
    public object? ErrorData { get; set; }

    public BpmnErrorEventException(string? message, string code) : base(message)
    {
        Code = code;
    }

    public BpmnErrorEventException(string? message, string code, object? errorData) : base(message)
    {
        Code = code;
        ErrorData = errorData;
    }
}