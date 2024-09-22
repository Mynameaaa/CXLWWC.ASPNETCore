namespace WWC._240711.ASPNETCore.Extensions;

public class CXLStringInstanceDefaultValueAttribute : CXLInstanceDefaultValueAttribute
{
    public string DefaultValue { get; set; }

    public CXLStringInstanceDefaultValueAttribute(string defaultValue)
    {
        DefaultValue = defaultValue;
    }
}
