namespace WWC._240711.ASPNETCore.TestAPI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CXLRouteAttribute : Attribute
    {
        public string Route { get; set; }

        public string ApiName { get; set; }

        public CXLRouteAttribute(string route)
        {
            Route = route;
        }

    }
}
