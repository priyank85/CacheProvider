using System.Globalization;
using Common.Logging;

namespace Provider.Cache
{
    public class NullObjectCacheAdapter : ICacheStorage
    {
        public NullObjectCacheAdapter()
        {
            LogManager.GetLogger<NullObjectCacheAdapter>()
                      .WarnFormat("Cache provider is set to: {0}", this.GetType().ToString());
        }

        public void Remove(string key)
        {
            // do nothing         
        }

        public void Store(string key, object data, System.Runtime.Caching.CacheItemPolicy itemPolicy)
        {
            // do nothing         
        }

        public T Retrieve<T>(string storageKey)
        {            
            return default(T);
        }
    }
}
