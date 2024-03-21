namespace ZeeBridge.Attributes;

public class ExecutionTimeoutAttribute(int executionTimeout) : Attribute
{
    internal TimeSpan ExecutionTimeout = TimeSpan.FromSeconds(executionTimeout);
}