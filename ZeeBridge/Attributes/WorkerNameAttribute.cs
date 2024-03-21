namespace ZeeBridge.Attributes;

public class WorkerNameAttribute(string workerName) : Attribute
{
    internal readonly string WorkerName = workerName;
}