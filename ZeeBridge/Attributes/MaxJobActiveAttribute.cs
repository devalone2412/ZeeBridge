namespace ZeeBridge.Attributes;

public class MaxJobActiveAttribute(int maxJobActive) : Attribute
{
    internal readonly int MaxJobActive = maxJobActive;
}