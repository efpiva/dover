using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Dover.Framework.Monad
{
    // based on http://devtalk.net/csharp/chained-null-checks-and-the-maybe-monad/
    public static class MaybeClass
    {
        public static V With<T, V>(this T t, Func<T, V> selector)
            where T : class
            where V : class
        {
            if (t == null) return null;
            return selector(t);
        }

        public static TResult Return<TInput,TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue) 
            where TInput: class
        {
            if (o == null) return failureValue;
            return evaluator(o);
        }

        public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator)
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o) ? o : null;
        }

        public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator)
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o) ? null : o;
        }

        public static TInput Do<TInput>(this TInput o, Action<TInput> action)
            where TInput : class
        {
            if (o == null) return null;
            action(o);
            return o;
        }
    }
}
