using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{

    public interface ICustomConfigurationProvider
    {

        bool TryGet(string key, out string? value);

        void Set(string key, string? value);

        IChangeToken GetReloadToken();

        void Load();

        IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath);
    }
}
