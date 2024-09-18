using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Filters.Custom
{
    /// <summary>
    /// ResultFilter 在页面加载前和释放前调用
    /// </summary>
    public class CXLResultFilter : Attribute, IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine("准备返回响应");
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("响应已成功返回");
        }
    }
}
