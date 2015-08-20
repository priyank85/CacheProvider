using System.Configuration;
using System.Runtime.Caching;

namespace Provider.Cache
{
    public class CacheProvider
    {
        private static ICacheStorage _cache;
        private static volatile CacheProvider _instance;
        private static readonly object _lockObj = new object();

        private CacheProvider()
        {
            switch (ConfigurationManager.AppSettings["CacheType"])
            {
                case "1":
                    _cache = new MemoryCacheAdapter();
                    break;
                case "2":
                    _cache = new NCacheAdapter();
                    break;
                default:
                    _cache = new NullObjectCacheAdapter();
                    break;
            }
        }

        public static CacheProvider Default
        {
            get
            {
                if (_instance == null)
                {
                    using (Lock.GetLock(_lockObj))
                    {
                        if (_instance == null)
                            _instance = new CacheProvider();
                    }
                }
                return _instance;
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Store(string key, object data, CacheItemPolicy itemPolicy)
        {
            _cache.Store(key, data, itemPolicy);
        }

        public T Retrieve<T>(string storageKey)
        {
            return _cache.Retrieve<T>(storageKey);
        }
    }
}
