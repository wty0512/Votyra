﻿using System;
using System.Collections.Generic;

namespace Votyra.Models.ObjectPool
{
    public class ObjectDictionaryPool<T, TKey> : BaseKeyObjectPool<T, TKey>
        where TKey : struct
    {
        private readonly Dictionary<TKey, List<T>> _objects;

        public ObjectDictionaryPool(int limit, Func<TKey, T> objectGenerator)
            : base(limit, objectGenerator)
        {
            _objects = new Dictionary<TKey, List<T>>();
        }

        protected override List<T> GetPool(TKey key)
        {
            List<T> objectPool;
            if (!_objects.TryGetValue(key, out objectPool))
            {
                objectPool = new List<T>();
                _objects[key] = objectPool;
            }
            return objectPool;
        }
    }
}