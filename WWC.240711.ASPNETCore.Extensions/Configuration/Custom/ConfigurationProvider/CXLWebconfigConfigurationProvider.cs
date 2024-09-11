using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using Microsoft.Extensions.FileProviders;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.Custom.ConfigurationProvider
{
    public class CXLWebconfigConfigurationProvider : IConfigurationProvider
    {
        private readonly string _filePath;
        private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

        public CXLWebconfigConfigurationProvider(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            return _data
                .Where(kv => kv.Key.StartsWith(parentPath + ConfigurationPath.KeyDelimiter))
                .Select(kv => kv.Key.Substring(parentPath.Length + 1))
                .Select(k => k.Split(ConfigurationPath.KeyDelimiter)[0])
                .Distinct();
        }

        public IChangeToken GetReloadToken()
        {
            // 如果需要支持文件变化监控，需要实现此方法
            return NullChangeToken.Singleton;
        }

        public void Load()
        {
            var doc = XDocument.Load(_filePath);

            foreach (var element in doc.Element("configuration").Elements())
            {
                ProcessElement(element, parentPath: string.Empty);
            }
        }

        private void ProcessElement(XElement element, string parentPath)
        {
            foreach (var child in element.Elements())
            {
                string path = string.IsNullOrEmpty(parentPath) ? child.Name.LocalName : $"{parentPath}:{child.Name.LocalName}";

                if (!child.HasElements && !child.HasAttributes)
                {
                    _data[path] = child.Value;
                }
                else
                {
                    ProcessElement(child, path);
                }

                if (child.HasAttributes)
                {
                    foreach (var attribute in child.Attributes())
                    {
                        _data[$"{path}:{attribute.Name.LocalName}"] = attribute.Value;
                    }
                }
            }
        }

        public void Set(string key, string value)
        {
            _data[key] = value;
        }

        public bool TryGet(string key, out string value)
        {
            return _data.TryGetValue(key, out value);
        }

        public void Reload()
        {
            Load();
        }
    }
}
