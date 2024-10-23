using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ocelot;
using Ocelot.Middleware;

namespace WWC._240711.ASPNETCore.Extensions.Exceptions.Custom
{
    /// <summary>
    /// 异常处理 Handler
    /// </summary>
    public class CXLOcelotLastExecptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public string logTemplate = @"{NewLine}时间：{CurrentTime}{NewLine}下游地址：{Path}{NewLine}{NewLine}请求体：{RequestBody}" + new string('-', 50);

        public CXLOcelotLastExecptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await Log500Error(context, ex);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                // 返回统一的错误码
                var errorResponse = new
                {
                    code = "500",
                    message = ex.ToString(),
                    showMessage = ex.Message
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }

            // 处理未捕获的请求错误
            if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
            {
                context.Response.ContentType = "application/json";
                var ocelotResponse = context.Items.DownstreamResponse();
                var requestBody = await ocelotResponse.Content.ReadAsStringAsync();
                var errorResponse = new
                {
                    code = "500",
                    showMessage = requestBody
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
        }

        /// <summary>
        /// 记录 500 异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private async Task Log500Error(HttpContext context, Exception exception)
        {
            string requestBody = string.Empty;
            DateTime currentTime = DateTime.Now;
            var ocelotResponse = context.Items.DownstreamResponse();
            var ocelotRequest = context.Items.DownstreamRequest();
            if (ocelotRequest.Method.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                requestBody = await ocelotResponse.Content.ReadAsStringAsync();
            }

            Log.Logger.Error(logTemplate,
                     currentTime, ocelotRequest.ToString(), string.IsNullOrEmpty(requestBody) ? "{}" : requestBody);
        }

    }
}
