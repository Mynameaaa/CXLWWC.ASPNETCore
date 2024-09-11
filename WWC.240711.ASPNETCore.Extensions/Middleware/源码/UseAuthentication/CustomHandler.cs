using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Middleware.源码.UseAuthentication
{
    public class CustomHandler : AuthenticationHandler<CustomAuthenticationOptions>
    {
        public CustomHandler(IOptionsMonitor<CustomAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (base.Options.Age > 18)
            {
                return Task.FromResult(AuthenticateResult.Success(null));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail(new Exception()));
            }
        }
    }
    public class CustomAuthenticationOptions : AuthenticationSchemeOptions
    {
        public int Age { get; set; }
    }
}
