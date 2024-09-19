using Microsoft.Extensions.Options;

namespace WWC._240711.ASPNETCore.Extensions
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
