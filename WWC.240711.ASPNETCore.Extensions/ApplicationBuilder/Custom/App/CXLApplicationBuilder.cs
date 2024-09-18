using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.ApplicationBuilder.Custom.App
{
    public class CXLApplicationBuilder : ICXLApplicationBuilder
    {
        private readonly IList<Func<CustomRequestDelegate, CustomRequestDelegate>> _middlewares = new List<Func<CustomRequestDelegate, CustomRequestDelegate>>();

        public CustomRequestDelegate Build()
        {
            CustomRequestDelegate next = context =>
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            };
            foreach (var middleware in _middlewares.Reverse())
            {
                next = middleware.Invoke(next);
            }
            return next;
        }

        public ICXLApplicationBuilder Use(Func<CustomRequestDelegate, CustomRequestDelegate> middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }
    }
}
