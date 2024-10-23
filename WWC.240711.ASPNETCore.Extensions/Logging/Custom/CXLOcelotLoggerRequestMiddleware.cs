using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocelot;
using Ocelot.Middleware;

namespace WWC._240711.ASPNETCore.Extensions.Logging.Custom
{
    public class CXLOcelotLoggerRequestMiddleware
    {
        private readonly RequestDelegate _next;
        public string logTemplate = @"{NewLine}时间：{CurrentTime}{NewLine}下游地址：{Path}{NewLine}路由：{Route}{NewLine}请求方法：{HttpMethod}{NewLine}" + new string('-', 50);

        public CXLOcelotLoggerRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            //if (context.Response.StatusCode == 200)
            //{
            //    Log200Info(context);
            //}
        }

        public void Log200Info(HttpContext context)
        {
            DateTime currentTime = DateTime.Now;
            var ocelotRequest = context.Items.DownstreamRequest();
            var path = ocelotRequest.ToUri();
            var route = ocelotRequest.AbsolutePath;
            var method = ocelotRequest.Method;

            Log.Information(logTemplate, currentTime, path, route, method);
        }
    }
}
