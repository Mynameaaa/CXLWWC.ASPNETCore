namespace WWC._240711.ASPNETCore.Extensions;

public interface ICXLServiceSubProvider : IDisposable
{
    public object GetService(Type interfaceType);

}
