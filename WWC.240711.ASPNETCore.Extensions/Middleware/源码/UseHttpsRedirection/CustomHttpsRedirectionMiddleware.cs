﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.HttpsPolicy
{
    /// <summary>
    /// Middleware that redirects non-HTTPS requests to an HTTPS URL.
    /// </summary>
    public class HttpsRedirectionMiddleware
    {
        private const int PortNotFound = -1;

        private readonly RequestDelegate _next;
        private readonly Lazy<int> _httpsPort;
        private readonly int _statusCode;

        private readonly IServerAddressesFeature? _serverAddressesFeature;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes <see cref="HttpsRedirectionMiddleware" />.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        /// <param name="config"></param>
        /// <param name="loggerFactory"></param>
        public HttpsRedirectionMiddleware(RequestDelegate next, IOptions<HttpsRedirectionOptions> options, IConfiguration config, ILoggerFactory loggerFactory)

        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var httpsRedirectionOptions = options.Value;
            if (httpsRedirectionOptions.HttpsPort.HasValue)
            {
                _httpsPort = new Lazy<int>(httpsRedirectionOptions.HttpsPort.Value);
            }
            else
            {
                _httpsPort = new Lazy<int>(TryGetHttpsPort);
            }
            _statusCode = httpsRedirectionOptions.RedirectStatusCode;
            _logger = loggerFactory.CreateLogger<HttpsRedirectionMiddleware>();
        }

        /// <summary>
        /// Initializes <see cref="HttpsRedirectionMiddleware" />.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        /// <param name="config"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="serverAddressesFeature"></param>
        public HttpsRedirectionMiddleware(RequestDelegate next, IOptions<HttpsRedirectionOptions> options, IConfiguration config, ILoggerFactory loggerFactory,
            IServerAddressesFeature serverAddressesFeature)
            : this(next, options, config, loggerFactory)
        {
            _serverAddressesFeature = serverAddressesFeature ?? throw new ArgumentNullException(nameof(serverAddressesFeature));
        }

        /// <summary>
        /// Invokes the HttpsRedirectionMiddleware.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            //如果当前请求是 Https 直接跳过
            if (context.Request.IsHttps)
            {
                return _next(context);
            }

            var port = _httpsPort.Value;
            if (port == PortNotFound)
            {
                return _next(context);
            }

            //如果请求的 Host 和当前 Host 不一致，返回跨域拦截
            var host = context.Request.Host;
            if (port != 443)
            {
                host = new HostString(host.Host, port);
            }
            else
            {
                host = new HostString(host.Host);
            }

            var request = context.Request;

            //重新拼装 Url
            var redirectUrl = UriHelper.BuildAbsolute(
                "https",
                host,
                request.PathBase,
                request.Path,
                request.QueryString);

            context.Response.StatusCode = _statusCode;
            //重定向到新的 Url[
            context.Response.Headers.Location = redirectUrl;

            //_logger.RedirectingToHttps(redirectUrl);
            Console.WriteLine("成功重定向到 Https");

            return Task.CompletedTask;
        }

        //  Returns PortNotFound (-1) if we were unable to determine the port.
        private int TryGetHttpsPort()
        {
            // The IServerAddressesFeature will not be ready until the middleware is Invoked,
            // Order for finding the HTTPS port:
            // 1. Set in the HttpsRedirectionOptions
            // 2. HTTPS_PORT environment variable
            // 3. IServerAddressesFeature
            // 4. Fail if not sets

            var nullablePort = _config.GetValue<int?>("HTTPS_PORT") ?? _config.GetValue<int?>("ANCM_HTTPS_PORT");
            if (nullablePort.HasValue)
            {
                var port = nullablePort.Value;
                //_logger.PortLoadedFromConfig(port);
                return port;
            }

            if (_serverAddressesFeature == null)
            {
                //_logger.FailedToDeterminePort();
                return PortNotFound;
            }

            foreach (var address in _serverAddressesFeature.Addresses)
            {
                var bindingAddress = BindingAddress.Parse(address);
                if (bindingAddress.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    // If we find multiple different https ports specified, throw
                    if (nullablePort.HasValue && nullablePort != bindingAddress.Port)
                    {
                        throw new InvalidOperationException(
                            "Cannot determine the https port from IServerAddressesFeature, multiple values were found. " +
                            "Set the desired port explicitly on HttpsRedirectionOptions.HttpsPort.");
                    }
                    else
                    {
                        nullablePort = bindingAddress.Port;
                    }
                }
            }

            if (nullablePort.HasValue)
            {
                var port = nullablePort.Value;
                //_logger.PortFromServer(port);
                return port;
            }

            //_logger.FailedToDeterminePort();
            return PortNotFound;
        }
    }
}
