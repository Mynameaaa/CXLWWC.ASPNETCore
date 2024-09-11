namespace WWC._240711.ASPNETCore.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomControllerAttribute : Attribute
    {

        public string GroupName { get; set; }

        public string DocName { get; set; }

    }
}
