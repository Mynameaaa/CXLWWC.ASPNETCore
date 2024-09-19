using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Server.Custom.App;
using WWC._240711.ASPNETCore.Extensions.Server.Custom.FeatureCollection;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class CustomHttpContext
    {
        public HttpRequest Request { get; }
        public HttpResponse Response { get; }
        public CustomHttpContext(ICustomFeatureCollection features)
        {
            Request = new HttpRequest(features);
            Response = new HttpResponse(features);
        }
    }

    public class HttpRequest
    {
        private readonly ICustomHttpRequestFeature _feature;
        public Uri Url => _feature.Url;
        public NameValueCollection Headers => _feature.Headers;
        public Stream Body => _feature.Body;
        public HttpRequest(ICustomFeatureCollection features) => _feature = features.Get<ICustomHttpRequestFeature>();
    }

    public class HttpResponse
    {
        private readonly ICustomHttpResponseFeature _feature;

        public NameValueCollection Headers => _feature.Headers;
        public Stream Body => _feature.Body;
        public int StatusCode
        {
            get => _feature.StatusCode;
            set => _feature.StatusCode = value;
        }
        public HttpResponse(ICustomFeatureCollection features) => _feature = features.Get<ICustomHttpResponseFeature>();
    }

    public static partial class Extensions
    {
        public static Task WriteAsync(this HttpResponse response, string contents)
        {
            var buffer = Encoding.UTF8.GetBytes(contents);
            return response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
