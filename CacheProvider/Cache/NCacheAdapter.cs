using System;
using System.Runtime.Caching;

namespace Provider.Cache
{
    public class NCacheAdapter : ICacheStorage
    {
        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Store(string key, object data, CacheItemPolicy itemPolicy)
        {
            throw new NotImplementedException();
        }

        public T Retrieve<T>(string storageKey)
        {
            throw new NotImplementedException();
        }
    }
}
