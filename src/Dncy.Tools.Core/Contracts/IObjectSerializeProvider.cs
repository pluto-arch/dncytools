﻿using System;

namespace Dotnetydd.Tools.Contracts
{
    public interface IObjectSerializeProvider
    {
        string Serialize(object obj);

        T Deserialize<T>(string objStr);

        object Deserialize(string objStr, Type type);
    }
}
