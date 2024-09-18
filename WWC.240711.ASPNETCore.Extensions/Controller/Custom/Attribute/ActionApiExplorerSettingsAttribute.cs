namespace WWC._240711.ASPNETCore.Extensions
{
    /// <summary>
    /// Action 标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ActionApiExplorerSettingsAttribute : Attribute
    {
        public bool IsHide { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isHide"></param>
        public ActionApiExplorerSettingsAttribute(bool isHide = false)
        {
            IsHide = isHide;
        }
    }
}
