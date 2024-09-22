using System.Net;
using WWC._240711.ASPNETCore.Extensions.Server.Custom.App;
using WWC._240711.ASPNETCore.Extensions.Server.Custom.FeatureCollection;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class CustomHttpListenerServer : ICustomServer
    {
        private readonly HttpListener _httpListener;
        private readonly string[] _urls;
        public CustomHttpListenerServer(params string[] urls)
        {
            _httpListener = new HttpListener();
            _urls = urls.Any() ? urls : new string[] { "http://localhost:5000/" };
        }

        public async Task StartAsync(CustomRequestDelegate handler)
        {
            Array.ForEach(_urls, url => _httpListener.Prefixes.Add(url));
            _httpListener.Start();
            while (true)
            {
                var listenerContext = await _httpListener.GetContextAsync();
                var feature = new CustomHttpListenerFeature(listenerContext);
                var features = new CustomFeatureCollection()
                    .Set<ICustomHttpRequestFeature>(feature)
                    .Set<ICustomHttpResponseFeature>(feature);
                var httpContext = new CustomHttpContext(features);
                await handler(httpContext);
                listenerContext.Response.Close();
            }
        }
    }
}
