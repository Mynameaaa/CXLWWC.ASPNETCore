using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// The root node for a configuration.
    /// </summary>
    public class CustomConfigurationRoot : ICustomConfigurationRoot, IDisposable
    {
        private readonly IList<ICustomConfigurationProvider> _providers;
        private readonly IList<IDisposable> _changeTokenRegistrations;
        private ConfigurationReloadToken _changeToken = new ConfigurationReloadToken();

        /// <summary>
        /// Initializes a Configuration root with a list of providers.
        /// </summary>
        /// <param name="providers">The <see cref="IConfigurationProvider"/>s for this configuration.</param>
        public CustomConfigurationRoot(IList<ICustomConfigurationProvider> providers)
        {
            if (providers == null)
            {
                throw new ArgumentNullException(nameof(providers));
            }

            _providers = providers;
            _changeTokenRegistrations = new List<IDisposable>(providers.Count);
            foreach (ICustomConfigurationProvider p in providers)
            {
                p.Load();
                _changeTokenRegistrations.Add(ChangeToken.OnChange(() => p.GetReloadToken(), () => RaiseChanged()));
            }
        }

        /// <summary>
        /// The <see cref="ICustomConfigurationProvider"/>s for this configuration.
        /// </summary>
        public IEnumerable<ICustomConfigurationProvider> Providers => _providers;

        /// <summary>
        /// Gets or sets the value corresponding to a configuration key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public string this[string key]
        {
            get => GetConfiguration(_providers, key);
            set => SetConfiguration(_providers, key, value);
        }

        /// <summary>
        /// Gets the immediate children sub-sections.
        /// </summary>
        /// <returns>The children.</returns>
        public IEnumerable<ICustomConfigurationSection> GetChildren() => null;

        /// <summary>
        /// Returns a <see cref="IChangeToken"/> that can be used to observe when this configuration is reloaded.
        /// </summary>
        /// <returns>The <see cref="IChangeToken"/>.</returns>
        public IChangeToken GetReloadToken() => _changeToken;

        /// <summary>
        /// Gets a configuration sub-section with the specified key.
        /// </summary>
        /// <param name="key">The key of the configuration section.</param>
        /// <returns>The <see cref="ICustomConfigurationSection"/>.</returns>
        /// <remarks>
        ///     This method will never return <c>null</c>. If no matching sub-section is found with the specified key,
        ///     an empty <see cref="ICustomConfigurationSection"/> will be returned.
        /// </remarks>
        public ICustomConfigurationSection GetSection(string key)
            => new CustomConfigurationSection(this, key);

        /// <summary>
        /// Force the configuration values to be reloaded from the underlying sources.
        /// </summary>
        public void Reload()
        {
            foreach (ICustomConfigurationProvider provider in _providers)
            {
                provider.Load();
            }
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            ConfigurationReloadToken previousToken = Interlocked.Exchange(ref _changeToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // dispose change token registrations
            foreach (IDisposable registration in _changeTokenRegistrations)
            {
                registration.Dispose();
            }

            // dispose providers
            foreach (IConfigurationProvider provider in _providers)
            {
                (provider as IDisposable)?.Dispose();
            }
        }

        internal static string GetConfiguration(IList<ICustomConfigurationProvider> providers, string key)
        {
            for (int i = providers.Count - 1; i >= 0; i--)
            {
                ICustomConfigurationProvider provider = providers[i];

                if (provider.TryGet(key, out string value))
                {
                    return value;
                }
            }

            return null;
        }

        internal static void SetConfiguration(IList<ICustomConfigurationProvider> providers, string key, string value)
        {
            if (providers.Count == 0)
            {
                throw new InvalidOperationException("不存在 provider");
            }

            foreach (IConfigurationProvider provider in providers)
            {
                provider.Set(key, value);
            }
        }
    }
}
