﻿using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DotnetGeek.Tools
{
    public class NullableDictionary<TKey, TValue> : Dictionary<NullObject<TKey>, TValue>
    {
        public NullableDictionary() : base()
        {
        }

        public NullableDictionary(int capacity) : base(capacity)
        {
        }

        public NullableDictionary(IEqualityComparer<NullObject<TKey>> comparer) : base(comparer)
        {
        }

        public NullableDictionary(int capacity, IEqualityComparer<NullObject<TKey>> comparer) : base(capacity, comparer)
        {
        }

        public NullableDictionary(IDictionary<NullObject<TKey>, TValue> dictionary) : base(dictionary)
        {
        }

        public NullableDictionary(IDictionary<NullObject<TKey>, TValue> dictionary, IEqualityComparer<NullObject<TKey>> comparer) : base(dictionary, comparer)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        public new TValue this[NullObject<TKey> key]
        {
            get => TryGetValue(key, out var value) ? value : default;
            set => base[key] = value!;
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="dic"></param>
        public static implicit operator NullableDictionary<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            var nullableDictionary = new NullableDictionary<TKey, TValue>();
            foreach (var p in dic)
            {
                nullableDictionary[p.Key] = p.Value;
            }
            return nullableDictionary;
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="dic"></param>
        public static implicit operator NullableDictionary<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dic)
        {
            var nullableDictionary = new NullableDictionary<TKey, TValue>();
            foreach (var p in dic)
            {
                nullableDictionary[p.Key] = p.Value;
            }
            return nullableDictionary;
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="dic"></param>
        public static implicit operator Dictionary<TKey, TValue>(NullableDictionary<TKey, TValue> dic)
        {
            var newdic = new Dictionary<TKey, TValue>();
            foreach (var p in dic)
            {
                newdic[p.Key] = p.Value;
            }
            return newdic;
        }
    }
}

