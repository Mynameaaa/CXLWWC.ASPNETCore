using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Server.Custom.App
{
    public class CustomHttpListenerFeature : ICustomHttpRequestFeature, ICustomHttpResponseFeature
    {
        private readonly HttpListenerContext _context;
        public CustomHttpListenerFeature(HttpListenerContext context) => _context = context;
        Uri ICustomHttpRequestFeature.Url => _context.Request.Url;
        NameValueCollection ICustomHttpRequestFeature.Headers => _context.Request.Headers;
        NameValueCollection ICustomHttpResponseFeature.Headers => _context.Response.Headers;
        Stream ICustomHttpRequestFeature.Body => _context.Request.InputStream;
        Stream ICustomHttpResponseFeature.Body => _context.Response.OutputStream;
        int ICustomHttpResponseFeature.StatusCode
        {
            get => _context.Response.StatusCode;
            set => _context.Response.StatusCode = value;
        }
    }
}
