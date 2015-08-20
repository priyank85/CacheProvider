using System.Runtime.Caching;

namespace Provider.Cache
{
    public interface ICacheStorage
    {
        void Remove(string key);
        void Store(string key, object data, CacheItemPolicy itemPolicy);
        T Retrieve<T>(string storageKey);
    }
}
