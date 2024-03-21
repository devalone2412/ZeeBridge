namespace ZeeBridge.Attributes;

public class PollIntervalAttribute(int pollInterval) : Attribute
{
    internal TimeSpan PollInterval = TimeSpan.FromSeconds(pollInterval);
}