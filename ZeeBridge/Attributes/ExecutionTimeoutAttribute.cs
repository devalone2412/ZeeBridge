namespace ZeeBridge.Attributes;

public class ExecutionTimeoutAttribute : Attribute
{
    internal TimeSpan ExecutionTimeout;

    public ExecutionTimeoutAttribute(int executionTimeout)
    {
        ExecutionTimeout = TimeSpan.FromSeconds(executionTimeout);
    }
}