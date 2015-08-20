using System;

namespace Provider.Cache
{
    public interface ICache<TKey, TValue> where TValue : class
    {
        TValue Get(TKey key, bool refresh, Func<TKey, TValue> sourceMethod);
        TValue Get(TKey key, Func<TKey, TValue> sourceMethod);
        void Remove(TKey key);
    }
}
