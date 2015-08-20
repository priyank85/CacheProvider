using System;
using System.Configuration;
using System.Globalization;
using System.Runtime.Caching;
using Common.Logging;

namespace Provider.Cache
{
    public enum CacheReturnStatus
    {
        Missing,
        NotFound,
        Loaded,
        Added,
        Found,
        Disabled,
        Replaced,
        Removed
    }

    public class Cache<TKey, TValue> : ICache<TKey,TValue> where TValue : class
    {
        /// <summary>
        /// Unit in Minutes.
        /// </summary>
        readonly int _defaultCacheTimeout;
        readonly string _cacheBucket;

        public Cache(string cacheBucket)
        {
            _cacheBucket = cacheBucket;

            int timeout;
            _defaultCacheTimeout = int.TryParse(ConfigurationManager.AppSettings["CacheItemTimout"], out timeout) ? timeout : 10;
        }

        public TValue Get(TKey key, Func<TKey, TValue> sourceMethod)
        {
            return Get(key, false, sourceMethod);
        }

        public TValue Get(TKey key, bool refresh, Func<TKey, TValue> sourceMethod)
        {
            var provider = CacheProvider.Default;
            var readFromCache = CacheReturnStatus.Added;

            var internalKey = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", _cacheBucket, key.ToString());

            var objProvider = provider.Retrieve<TValue>(internalKey);
            if (objProvider == null || refresh)
            {
                if (sourceMethod != null)
                {
                    objProvider = sourceMethod(key);

                    if (objProvider != null)
                    {
                        var exist = provider.Retrieve<TValue>(internalKey);
                        if (exist == null)
                            readFromCache = CacheReturnStatus.Added;
                        else
                            readFromCache = CacheReturnStatus.Replaced;

                        var policy = new CacheItemPolicy
                                         {
                                             Priority = CacheItemPriority.Default,
                                             AbsoluteExpiration = DateTime.Now.AddMinutes(_defaultCacheTimeout),
                                             RemovedCallback = new CacheEntryRemovedCallback(CacheItemRemoved)
                                         };

                        provider.Store(internalKey, objProvider, policy);
                    }
                    else
                        readFromCache = CacheReturnStatus.NotFound;
                }
                else
                    readFromCache = CacheReturnStatus.Missing;
            }
            else
                readFromCache = CacheReturnStatus.Found;

            LogManager.GetLogger<Cache<TKey, TValue>>()
                      .InfoFormat("Cache {0}: {1} {2}", _cacheBucket, key, readFromCache.ToString());

            return objProvider;
        }

        private void CacheItemRemoved(CacheEntryRemovedArguments args)
        {
            LogManager.GetLogger<Cache<TKey, TValue>>()
                      .InfoFormat("Cache {0}: {1}, Reason: {2}", args.CacheItem.Key,
                CacheReturnStatus.Removed.ToString(), args.RemovedReason.ToString());
        }

        public void Remove(TKey key)
        {
            var provider = CacheProvider.Default;
            var internalKey = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", _cacheBucket, key);
            provider.Remove(internalKey);
        }
    }
}