namespace WWC._240711.ASPNETCore.Extensions
{
    public delegate Task CustomRequestDelegate(CustomHttpContext context);

    public interface ICXLApplicationBuilder
    {
        CustomRequestDelegate Build();
        ICXLApplicationBuilder Use(Func<CustomRequestDelegate, CustomRequestDelegate> middleware);

    }
}
