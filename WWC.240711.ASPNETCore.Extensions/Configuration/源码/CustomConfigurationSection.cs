using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    public class CustomConfigurationSection : ICustomConfigurationSection
    {

        private readonly ICustomConfigurationRoot _root;
        private readonly string _path;
        private string _key;

        /// <summary>
        /// 初始化新实例。
        /// </summary>
        /// <param name="root">The configuration root.</param>
        /// <param name="path">The path to this section.</param>
        public CustomConfigurationSection(ICustomConfigurationRoot root, string path)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            _root = root;
            _path = path;
        }

        /// <summary>
        /// 从<see cref=“IConfigurationRoot”/>获取此节的完整路径。
        /// </summary>
        public string Path => _path;

        /// <summary>
        /// 获取此节在其父节中占用的键。
        /// </summary>
        public string Key
        {
            get
            {
                if (_key == null)
                {
                    // Key被延迟计算为Path的最后一部分
                    _key = ConfigurationPath.GetSectionKey(_path);
                }
                return _key;
            }
        }

        /// <summary>
        /// 获取或设置节值。
        /// </summary>
        public string Value
        {
            get
            {
                return _root[Path];
            }
            set
            {
                _root[Path] = value;
            }
        }

        /// <summary>
        /// 获取或设置与配置键对应的值。
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public string this[string key]
        {
            get
            {
                return _root[ConfigurationPath.Combine(Path, key)];
            }

            set
            {
                _root[ConfigurationPath.Combine(Path, key)] = value;
            }
        }

        /// <summary>
        /// 获取具有指定键的配置子节。
        /// </summary>
        /// <param name="key">配置部分的关键。</param>
        /// <returns>The <see cref="IConfigurationSection"/>.</returns>
        /// <remarks>
        ///     此方法永远不会返回 <c>null</c>。如果没有找到与指定密钥匹配的子部分，
        ///     将返回一个空的<see-cref=“IConfigurationSection”/>。
        /// </remarks>
        public ICustomConfigurationSection GetSection(string key) => _root.GetSection(ConfigurationPath.Combine(Path, key));

        /// <summary>
        /// 获取直接子代配置子节。
        /// </summary>
        /// <returns>配置子部分。</returns>
        public IEnumerable<ICustomConfigurationSection> GetChildren() => null;

        /// <summary>
        /// 返回一个<see cref=“IChangeToken”/>，可用于观察此配置何时重新加载。
        /// </summary>
        /// <returns>The <see cref="IChangeToken"/>.</returns>
        public IChangeToken GetReloadToken() => _root.GetReloadToken();
    }
}
