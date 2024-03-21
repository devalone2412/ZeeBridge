namespace ZeeBridge.Attributes;

public class PollIntervalAttribute : Attribute
{
    internal TimeSpan PollInterval;

    public PollIntervalAttribute(int pollInterval)
    {
        PollInterval = TimeSpan.FromSeconds(pollInterval);
    }
}