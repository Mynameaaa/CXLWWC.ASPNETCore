using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Http.Custom;

namespace WWC._240711.ASPNETCore.Extensions.Options.Custom
{
    public class NamedHttpClientConfigureOptions : IConfigureOptions<List<NamedHttpClientOptions>>
    {
        private INamedOptionsService _namedOptionsService;

        public NamedHttpClientConfigureOptions(INamedOptionsService namedOptionsService)
        {
            _namedOptionsService = namedOptionsService;
        }

        public void Configure(List<NamedHttpClientOptions> options)
        {
            options = _namedOptionsService.GetHttpClientOptions().Result ?? null;
        }
    }
}
