using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WWC._240711.ASPNETCore.TestAPI
{
    /// <summary>
    /// 控制方法的加载，与 Swagger 的隐藏不同这是直接不会加载，无法调用，Swagger 还可以调用
    /// </summary>
    public class ApiExplorerHideOnlyConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            var attribute = action.Attributes.OfType<ActionApiExplorerSettingsAttribute>()?.FirstOrDefault();
            if (attribute == null)
                return;

            action.ApiExplorer.IsVisible = !attribute.IsHide;
        }
    }
}
