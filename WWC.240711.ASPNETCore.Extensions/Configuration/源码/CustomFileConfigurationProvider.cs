using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{

    /// <summary>
    /// 基于文件的基类<参见cref=“ConfigurationProvider”/>。
    /// </summary>
    public abstract class CustomFileConfigurationProvider : ICustomConfigurationProvider, IDisposable
    {
        private readonly IDisposable _changeTokenRegistration;

        /// <summary>
        /// 使用指定的源初始化新实例。
        /// </summary>
        /// <param name="source">The source settings.</param>
        public CustomFileConfigurationProvider(CustomFileConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));

            if (Source.ReloadOnChange && Source.FileProvider != null)
            {
                _changeTokenRegistration = ChangeToken.OnChange(
                    () => Source.FileProvider.Watch(Source.Path),
                    () =>
                    {
                        Thread.Sleep(Source.ReloadDelay);
                        Load(reload: true);
                    });
            }
        }

        /// <summary>
        /// 此提供程序的源设置。
        /// </summary>
        public CustomFileConfigurationSource Source { get; }

        /// <summary>
        /// 生成一个表示此提供程序名称和相关详细信息的字符串。
        /// </summary>
        /// <returns> The configuration name. </returns>
        public override string ToString()
            => $"{GetType().Name} for '{Source.Path}' ({(Source.Optional ? "Optional" : "Required")})";

        private void Load(bool reload)
        {
            IFileInfo file = Source.FileProvider?.GetFileInfo(Source.Path);
            if (file == null || !file.Exists)
            {
                if (Source.Optional || reload) //重新加载或者可选
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    var error = new StringBuilder("异常");
                    if (!string.IsNullOrEmpty(file?.PhysicalPath))
                    {
                        error.Append("异常");
                    }
                    HandleException(ExceptionDispatchInfo.Capture(new FileNotFoundException(error.ToString())));
                }
            }
            else
            {
                static Stream OpenRead(IFileInfo fileInfo)
                {
                    if (fileInfo.PhysicalPath != null)
                    {
                        // The default physical file info assumes asynchronous IO which results in unnecessary overhead
                        // especially since the configuration system is synchronous. This uses the same settings
                        // and disables async IO.
                        return new FileStream(
                            fileInfo.PhysicalPath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite,
                            bufferSize: 1,
                            FileOptions.SequentialScan);
                    }

                    return fileInfo.CreateReadStream();
                }

                using Stream stream = OpenRead(file);
                try
                {
                    Load(stream);
                }
                catch (Exception ex)
                {
                    if (reload)
                    {
                        Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }
                    var exception = new InvalidDataException("异常", ex);
                    HandleException(ExceptionDispatchInfo.Capture(exception));
                }
            }
            // REVIEW: Should we raise this in the base as well / instead?
            OnReload();
        }

        /// <summary>
        /// Loads the contents of the file at <see cref="Path"/>.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">If Optional is <c>false</c> on the source and part of a file or
        /// or directory cannot be found at the specified Path.</exception>
        /// <exception cref="FileNotFoundException">If Optional is <c>false</c> on the source and a
        /// file does not exist at specified Path.</exception>
        /// <exception cref="InvalidDataException">Wrapping any exception thrown by the concrete implementation of the
        /// <see cref="Load()"/> method. Use the source <see cref="FileConfigurationSource.OnLoadException"/> callback
        /// if you need more control over the exception.</exception>
        public void Load()
        {
            Load(reload: false);
        }

        /// <summary>
        /// Loads this provider's data from a stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        public abstract void Load(Stream stream);

        private void HandleException(ExceptionDispatchInfo info)
        {
            bool ignoreException = false;
            if (Source.OnLoadException != null)
            {
                var exceptionContext = new FileLoadExceptionContext
                {
                    Provider = default(FileConfigurationProvider),
                    Exception = info.SourceException
                };
                Source.OnLoadException.Invoke(exceptionContext);
                ignoreException = exceptionContext.Ignore;
            }
            if (!ignoreException)
            {
                info.Throw();
            }
        }

        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Dispose the provider.
        /// </summary>
        /// <param name="disposing"><c>true</c> if invoked from <see cref="IDisposable.Dispose"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            _changeTokenRegistration?.Dispose();
        }


        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

        /// <summary>
        /// The configuration key value pairs for this provider.
        /// </summary>
        protected IDictionary<string, string> Data { get; set; }

        /// <summary>
        /// Attempts to find a value with the given key, returns true if one is found, false otherwise.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <param name="value">The value found at key if one is found.</param>
        /// <returns>True if key has a value, false otherwise.</returns>
        public virtual bool TryGet(string key, out string value)
            => Data.TryGetValue(key, out value);

        /// <summary>
        /// Sets a value for a given key.
        /// </summary>
        /// <param name="key">The configuration key to set.</param>
        /// <param name="value">The value to set.</param>
        public virtual void Set(string key, string value)
            => Data[key] = value;

        /// <summary>
        /// Returns the list of keys that this provider has.
        /// </summary>
        /// <param name="earlierKeys">The earlier keys that other providers contain.</param>
        /// <param name="parentPath">The path for the parent IConfiguration.</param>
        /// <returns>The list of keys for this provider.</returns>
        public virtual IEnumerable<string> GetChildKeys(
            IEnumerable<string> earlierKeys,
            string parentPath)
        {
            var results = new List<string>();

            if (parentPath is null)
            {
                foreach (KeyValuePair<string, string> kv in Data)
                {
                    results.Add(Segment(kv.Key, 0));
                }
            }
            else
            {
                Debug.Assert(ConfigurationPath.KeyDelimiter == ":");

                foreach (KeyValuePair<string, string> kv in Data)
                {
                    if (kv.Key.Length > parentPath.Length &&
                        kv.Key.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) &&
                        kv.Key[parentPath.Length] == ':')
                    {
                        results.Add(Segment(kv.Key, parentPath.Length + 1));
                    }
                }
            }

            results.AddRange(earlierKeys);

            results.Sort(ConfigurationKeyComparer.Instance);

            return results;
        }

        private static string Segment(string key, int prefixLength)
        {
            int indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }

        /// <summary>
        /// Returns a <see cref="IChangeToken"/> that can be used to listen when this provider is reloaded.
        /// </summary>
        /// <returns>The <see cref="IChangeToken"/>.</returns>
        public IChangeToken GetReloadToken()
        {
            return _reloadToken;
        }

        /// <summary>
        /// Triggers the reload change token and creates a new one.
        /// </summary>
        protected void OnReload()
        {
            ConfigurationReloadToken previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }

    }

}
