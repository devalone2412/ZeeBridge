namespace ZeeBridge.Exceptions;

public class InvalidBpmnException(string? message, Exception? innerException) : Exception(message, innerException);