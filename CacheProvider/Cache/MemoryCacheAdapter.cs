using System.Runtime.Caching;
using System.Globalization;
using Common.Logging;

namespace Provider.Cache
{
    public class MemoryCacheAdapter : ICacheStorage
    {
        public MemoryCacheAdapter()
        {
            LogManager.GetLogger<MemoryCacheAdapter>()
                      .WarnFormat("Cache provider is set to: {0}", this.GetType().ToString());
        }

        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        public void Store(string key, object data, CacheItemPolicy itemPolicy)
        {
            //MemoryCache.Default.Add(key, data, itemPolicy);
            MemoryCache.Default.Set(key, data, itemPolicy);
        }

        public T Retrieve<T>(string storageKey)
        {
            T itemStored = (T)MemoryCache.Default.Get(storageKey);
            if (itemStored == null)
                itemStored = default(T);

            return itemStored;
        } 
    }
}
