﻿using System;
using System.Collections.Generic;

namespace DotnetGeek.Tools
{
    public class DisposeableDictionary<TKey, TValue> : NullableDictionary<TKey, TValue>, IDisposable where TValue : IDisposable
    {
        private bool _isDisposed;

        /// <summary>
        /// 终结器
        /// </summary>
        ~DisposeableDictionary()
        {
            Dispose(false);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            Dispose(true);
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public DisposeableDictionary() : base()
        {
        }

        public DisposeableDictionary(int capacity) : base(capacity)
        {
        }

        public DisposeableDictionary(IDictionary<NullObject<TKey>, TValue> dictionary) : base(dictionary)
        {
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            foreach (var s in Values)
            {
                s?.Dispose();
            }
        }
    }
}

