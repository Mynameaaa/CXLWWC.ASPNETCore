using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Filters.Custom
{
    internal class CXLResourceFliter : IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            Console.WriteLine("准备开始进入方法执行流程");
            await next();
            Console.WriteLine("方法流程与试图流程均已执行");
        }
    }
}
