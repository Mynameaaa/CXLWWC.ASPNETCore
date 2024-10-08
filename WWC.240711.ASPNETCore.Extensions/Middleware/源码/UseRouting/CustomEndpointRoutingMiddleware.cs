﻿//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.

//using System;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Routing.Matching;
//using Microsoft.Extensions.FileSystemGlobbing;
//using Microsoft.Extensions.Logging;

//namespace Microsoft.AspNetCore.Routing
//{
//    internal sealed partial class EndpointRoutingMiddleware
//    {
//        private const string DiagnosticsEndpointMatchedKey = "Microsoft.AspNetCore.Routing.EndpointMatched";

//        private readonly MatcherFactory _matcherFactory;
//        private readonly ILogger _logger;
//        private readonly EndpointDataSource _endpointDataSource;
//        private readonly DiagnosticListener _diagnosticListener;
//        private readonly RequestDelegate _next;

//        private Task<Matcher>? _initializationTask;

//        public EndpointRoutingMiddleware(
//            MatcherFactory matcherFactory,
//            ILogger<EndpointRoutingMiddleware> logger,
//            IEndpointRouteBuilder endpointRouteBuilder,
//            DiagnosticListener diagnosticListener,
//            RequestDelegate next)
//        {
//            if (endpointRouteBuilder == null)
//            {
//                throw new ArgumentNullException(nameof(endpointRouteBuilder));
//            }

//            _matcherFactory = matcherFactory ?? throw new ArgumentNullException(nameof(matcherFactory));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _diagnosticListener = diagnosticListener ?? throw new ArgumentNullException(nameof(diagnosticListener));
//            _next = next ?? throw new ArgumentNullException(nameof(next));

//            _endpointDataSource = new CompositeEndpointDataSource(endpointRouteBuilder.DataSources);
//        }

//        public Task Invoke(HttpContext httpContext)
//        {
//            //已经有一个端点，则跳过匹配
//            var endpoint = httpContext.GetEndpoint();
//            if (endpoint != null)
//            {
//                Log.MatchSkipped(_logger, endpoint);
//                return _next(httpContext);
//            }

//            // 等待init和访问匹配器之间存在固有的竞争条件
//            // 这是可以的，因为一旦初始化了`_matcher`，它就不会再被设置为null。
//            var matcherTask = InitializeAsync();
//            if (!matcherTask.IsCompletedSuccessfully)
//            {
//                return AwaitMatcher(this, httpContext, matcherTask);
//            }

//            var matchTask = matcherTask.Result.MatchAsync(httpContext);
//            if (!matchTask.IsCompletedSuccessfully)
//            {
//                return AwaitMatch(this, httpContext, matchTask);
//            }

//            return SetRoutingAndContinue(httpContext);

//            // 等待任务未同步完成时的回退
//            static async Task AwaitMatcher(EndpointRoutingMiddleware middleware, HttpContext httpContext, Task<Matcher> matcherTask)
//            {
//                var matcher = await matcherTask;
//                await matcher.MatchAsync(httpContext);
//                await middleware.SetRoutingAndContinue(httpContext);
//            }

//            static async Task AwaitMatch(EndpointRoutingMiddleware middleware, HttpContext httpContext, Task matchTask)
//            {
//                await matchTask;
//                await middleware.SetRoutingAndContinue(httpContext);
//            }

//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private Task SetRoutingAndContinue(HttpContext httpContext)
//        {
//            // 如果端点没有突变，则记录失败
//            var endpoint = httpContext.GetEndpoint();
//            if (endpoint == null)
//            {
//                Log.MatchFailure(_logger);
//            }
//            else
//            {
//                // 如果路线匹配，则发起事件
//                if (_diagnosticListener.IsEnabled() && _diagnosticListener.IsEnabled(DiagnosticsEndpointMatchedKey))
//                {
//                    // 我们只发送HttpContext，因为它包含了所有相关信息
//                    _diagnosticListener.Write(DiagnosticsEndpointMatchedKey, httpContext);
//                }

//                Log.MatchSuccess(_logger, endpoint);
//            }

//            return _next(httpContext);
//        }

//        // Initialization is async to avoid blocking threads while reflection and things
//        // of that nature take place.
//        //
//        // We've seen cases where startup is very slow if we  allow multiple threads to race
//        // while initializing the set of endpoints/routes. Doing CPU intensive work is a
//        // blocking operation if you have a low core count and enough work to do.
//        private Task<Matcher> InitializeAsync()
//        {
//            var initializationTask = _initializationTask;
//            if (initializationTask != null)
//            {
//                return initializationTask;
//            }

//            return InitializeCoreAsync();
//        }

//        private Task<Matcher> InitializeCoreAsync()
//        {
//            var initialization = new TaskCompletionSource<Matcher>(TaskCreationOptions.RunContinuationsAsynchronously);
//            var initializationTask = Interlocked.CompareExchange(ref _initializationTask, initialization.Task, null);
//            if (initializationTask != null)
//            {
//                // This thread lost the race, join the existing task.
//                return initializationTask;
//            }

//            // This thread won the race, do the initialization.
//            try
//            {
//                var matcher = _matcherFactory.CreateMatcher(_endpointDataSource);

//                _initializationTask = Task.FromResult(matcher);

//                // Complete the task, this will unblock any requests that came in while initializing.
//                initialization.SetResult(matcher);
//                return initialization.Task;
//            }
//            catch (Exception ex)
//            {
//                // Allow initialization to occur again. Since DataSources can change, it's possible
//                // for the developer to correct the data causing the failure.
//                _initializationTask = null;

//                // Complete the task, this will throw for any requests that came in while initializing.
//                initialization.SetException(ex);
//                return initialization.Task;
//            }
//        }

//        private static partial class Log
//        {
//            public static void MatchSuccess(ILogger logger, Endpoint endpoint)
//                => MatchSuccess(logger, endpoint.DisplayName);

//            [LoggerMessage(1, LogLevel.Debug, "Request matched endpoint '{EndpointName}'", EventName = "MatchSuccess")]
//            private static partial void MatchSuccess(ILogger logger, string? endpointName);

//            [LoggerMessage(2, LogLevel.Debug, "Request did not match any endpoints", EventName = "MatchFailure")]
//            public static partial void MatchFailure(ILogger logger);

//            public static void MatchSkipped(ILogger logger, Endpoint endpoint)
//                => MatchingSkipped(logger, endpoint.DisplayName);

//            [LoggerMessage(3, LogLevel.Debug, "Endpoint '{EndpointName}' already set, skipping route matching.", EventName = "MatchingSkipped")]
//            private static partial void MatchingSkipped(ILogger logger, string? endpointName);
//        }
//    }
//}
