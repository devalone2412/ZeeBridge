namespace ZeeBridge.Attributes;

public class WorkerNameAttribute : Attribute
{
    internal readonly string WorkerName;

    public WorkerNameAttribute(string workerName)
    {
        WorkerName = workerName;
    }
}