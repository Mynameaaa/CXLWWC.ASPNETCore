using Microsoft.AspNetCore.Diagnostics;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class CXLDeveloperPageExceptionFilter : IDeveloperPageExceptionFilter
    {
        public async Task HandleExceptionAsync(ErrorContext errorContext, Func<ErrorContext, Task> next)
        {
            await next(errorContext);
        }
    }
}
