namespace WWC._240711.ASPNETCore.Auth;

//使用这个标记的 Handler 如果在没有找到 Scheme 的情况下将使用该 Handler
[AttributeUsage(AttributeTargets.Class)]
public class NoSchemeDefaultHandlerAttribute : Attribute
{
}
