using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace StronglyTypedIdDemo.Infrastructure
{
    public static class StronglyTypedIdHelper
    {
        private static readonly ConcurrentDictionary<Type, Delegate> StronglyTypedIdFactories = new();

        public static bool IsStronglyTypedId(Type type) => IsStronglyTypedId(type, out _);

        public static bool IsStronglyTypedId(Type type, [NotNullWhen(true)] out Type idType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var t = type;
            while (t != null && t != typeof(object))
            {
                if (t.IsGenericType &&
                   t.GetGenericTypeDefinition() == typeof(StronglyTypedId<>))
                {

                    idType = t.GetGenericArguments()[0];
                    return true;
                }
                else
                {
                    t = t.BaseType;
                }
            }


            idType = null;
            return false;
        }

        public static Func<TValue, object> GetFactory<TValue>(Type stronglyTypedIdType) where TValue : notnull
            => (Func<TValue, object>)StronglyTypedIdFactories.GetOrAdd(stronglyTypedIdType, CreateFactory<TValue>);

        private static Func<TValue, object> CreateFactory<TValue>(Type stronglyTypedIdType) where TValue : notnull
        {
            if (!IsStronglyTypedId(stronglyTypedIdType))
                throw new ArgumentException($"Type '{stronglyTypedIdType}' is not a strongly-typed id type", nameof(stronglyTypedIdType));

            var ctor = stronglyTypedIdType.GetConstructor(new[] { typeof(TValue) });
            if (ctor is null)
                throw new ArgumentException($"Type '{stronglyTypedIdType}' doesn't have a constructor with one parameter of type '{typeof(TValue)}'", nameof(stronglyTypedIdType));

            var param = Expression.Parameter(typeof(TValue), "value");
            var body = Expression.New(ctor, param);
            var lambda = Expression.Lambda<Func<TValue, object>>(body, param);
            return lambda.Compile();
        }
    }
}
