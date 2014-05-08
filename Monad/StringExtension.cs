using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.Monad
{
    public static class StringExtension
    {
        public static String Left(this String s, Int32 length)
        {
            if (s.Length <= length)
                return s;

            return s.Substring(0, length);
        }

        public static String Right(this String s, Int32 length)
        {
            if (s.Length <= length)
                return s;
            return s.Substring(s.Length - length, length);
        }

        public static String Truncate(this String s, Int32 length)
        {
            return s.Length > length ? s.Substring(0, length) : s;
        }

    }
}
