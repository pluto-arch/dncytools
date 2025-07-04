using System;

namespace DotnetGeek.Tools
{
    /// <summary>
    /// 方法返回值包装器
    /// </summary>
    /// <remarks>避免方法执行尽用抛异常方式进行错误返回</remarks>
    /// <typeparam name="T">返回数据类型</typeparam>
    /// <typeparam name="E">错误信息类型</typeparam>
    public record Result<T, E>
    {
        private readonly bool _success;
        private readonly T value;
        private readonly E error;

        private Result(T v, E e, bool success)
        {
            value = v;
            error = e;
            _success = success;
        }


        public bool Successed => _success;


        public T Data => value;

        public E Errors => error;


        public static Result<T, E> Success(T v)
        {
            return new(v, default, true);
        }

        public static Result<T, E> Error(E e)
        {
            return new(default, e, false);
        }


        public static implicit operator Result<T, E>(T v) => new(v, default, true);
        public static implicit operator Result<T, E>(E e) => new(default, e, false);

        public R Match<R>(Func<T, R> success, Func<E, R> failure)
            => _success ? success(value) : failure(error);
    }


    /// <summary>
    /// 方法返回值包装器
    /// </summary>
    /// <remarks>避免方法执行尽用抛异常方式进行错误返回</remarks>
    /// <typeparam name="T">返回数据类型</typeparam>
    public record Result<T>
    {
        private readonly bool _success;
        private readonly T value;
        private readonly string error;

        private Result(T v, string e, bool success)
        {
            value = v;
            error = e;
            _success = success;
        }


        public bool Successed => _success;


        public T Data => value;

        public string Errors => error;


        public static Result<T> Success(T v)
        {
            return new(v, default, true);
        }

        public static Result<T> Error(string e)
        {
            return new(default, e, false);
        }


        public static implicit operator Result<T>(T v) => new(v, default, true);
        public static implicit operator Result<T>(string e) => new(default, e, false);

        public R Match<R>(Func<T, R> success, Func<string, R> failure)
            => _success ? success(value) : failure(error);
    }


    /// <summary>
    /// 方法返回值包装器
    /// </summary>
    /// <remarks>避免方法执行尽用抛异常方式进行错误返回</remarks>
    public record Result
    {
        private readonly bool _success;
        private readonly string error;
        private Result(string e, bool success)
        {
            error = e;
            _success = success;
        }
        public bool Successed => _success;
        public string Errors => error;
        public static Result Error(string e)
        {
            return new(e, false);
        }
        public static Result Success()
        {
            return new(default!, true);
        }

        public static implicit operator Result(string e) => new(e, false);

        public R Match<R>(Func<R> success, Func<string, R> failure)
            => _success ? success() : failure(error);
    }
}
