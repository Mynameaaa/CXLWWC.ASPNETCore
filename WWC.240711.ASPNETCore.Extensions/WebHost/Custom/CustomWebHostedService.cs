using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.ApplicationBuilder.Custom.App;
using WWC._240711.ASPNETCore.Extensions.Server.Custom;

namespace WWC._240711.ASPNETCore.Extensions.WebHost.Custom
{
    public class CustomWebHostedService : IHostedService
    {
        private readonly ICustomServer _server;
        private readonly CustomRequestDelegate _handler;
        public CustomWebHostedService(ICustomServer server, CustomRequestDelegate handler)
        {
            _server = server;
            _handler = handler;
        }

        public Task StartAsync(CancellationToken cancellationToken) => _server.StartAsync(_handler);
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
