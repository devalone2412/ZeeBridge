namespace ZeeBridge.Attributes;

public class MaxJobActiveAttribute : Attribute
{
    internal readonly int MaxJobActive;

    public MaxJobActiveAttribute(int maxJobActive)
    {
        MaxJobActive = maxJobActive;
    }
}