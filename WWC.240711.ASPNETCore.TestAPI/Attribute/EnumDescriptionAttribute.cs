namespace WWC._240711.ASPNETCore.TestAPI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }

    }
}
