using HyperTaskCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyperTaskCore.Utils
{
    public static class NumberHelper
    {
        public static bool IsBetween(this int value, int number1, int number2, bool inclusive = true)
        {
            int min = Math.Min(number1, number2);
            int max = Math.Max(number1, number2);

            if (inclusive)
            {
                return min <= value && value <= max;
            }
            else
            {
                return min < value && value < max;
            }
        }

        public static bool IsBetween(this TaskPosition value, int number1, int number2, bool inclusive = true)
        {
            int min = Math.Min(number1, number2);
            int max = Math.Max(number1, number2);

            if (inclusive)
            {
                return min <= value && value <= max;
            }
            else
            {
                return min < value && value < max;
            }
        }

        /// <summary>
        /// [Custom Extension] : Returns 0 or defaultLong if conversion fails
        /// </summary>
        public static long ToLongNoException(this Object number, long _defaultLong = 0)
        {
            long myLong = _defaultLong;

            if (number == DBNull.Value) return 0;

            try
            {
                myLong = Convert.ToInt64(number);
            }
            catch { }

            return myLong;
        }

        /// <summary>
        /// [Custom Extension] : Returns null or defaultLong if conversion fails
        /// </summary>
        public static long? ToLongNoExceptionNullable(this Object number, long? _defaultLong = null)
        {
            long? myLong = _defaultLong;

            if (number == DBNull.Value) return null;

            try
            {
                myLong = Convert.ToInt64(number);
            }
            catch { }

            return myLong;
        }
    }
}
