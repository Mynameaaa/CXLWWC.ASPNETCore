namespace WWC._240711.ASPNETCore.Extensions;

public class RegisterIDAttribute : Attribute
{
    public CXLServiceLifetime Lifetime { get; init; }

    public RegisterIDAttribute(CXLServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}
