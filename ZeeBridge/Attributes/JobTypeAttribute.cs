namespace ZeeBridge.Attributes;

public class JobTypeAttribute : Attribute
{
    internal readonly string JobType;

    public JobTypeAttribute(string jobType)
    {
        JobType = jobType;
    }
}