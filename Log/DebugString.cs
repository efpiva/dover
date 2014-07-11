using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dover.Framework.Log
{
    public static class DebugString
    {
        public static Func<string> Format(String msg, params object[] values)
        {
            Func<string> debugFunc = () =>
            {
                if (values == null || values.Length == 0)
                    return msg;

                return String.Format(msg, values);
            };
            return debugFunc;
        }
    }
}
