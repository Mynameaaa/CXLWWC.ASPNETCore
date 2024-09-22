namespace WWC._240711.ASPNETCore.Extensions;

[AttributeUsage(AttributeTargets.Parameter)]
public class CXLInstanceDefaultValueAttribute : Attribute
{
    public bool UseContainerService { get; set; } = false;

    public CXLInstanceDefaultValueAttribute()
    {
        
    }
}
