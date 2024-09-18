namespace WWC._240711.ASPNETCore.Extensions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CXLApiExplorerSettingsAttribute : Attribute
    {
        public string TagName { get; }

        public string GroupName { get; set; }

        public CXLApiExplorerSettingsAttribute(string tagName, string groupName)
        {
            TagName = tagName;
            GroupName = groupName;
        }
    }
}
