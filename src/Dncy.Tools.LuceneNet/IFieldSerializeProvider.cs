using System;

namespace Dncy.Tools.LuceneNet
{
    public interface IFieldSerializeProvider
    {
        string Serialize(object obj);


        T? Deserialize<T>(string objStr);


        object? Deserialize(string objStr, Type type);
    }
}

