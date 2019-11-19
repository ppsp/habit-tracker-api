using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Utils
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
    }
}
