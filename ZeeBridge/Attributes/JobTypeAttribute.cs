namespace ZeeBridge.Attributes;

public class JobTypeAttribute(string jobType) : Attribute
{
    internal readonly string JobType = jobType;
}