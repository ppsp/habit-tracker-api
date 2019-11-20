using HabitTrackerCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    /// <summary>
    /// A TaskPosition acts like an integer but it cannot be below 1 or above 500
    /// </summary>
    public class TaskPosition
    {
        private int _value { get; set; }
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value < 1)
                    throw new TaskPositionInvalidException("TaskPosition cannot be smaller than one");
                else if (_value > 500)
                    throw new TaskPositionInvalidException("TaskPosition cannot be greater than 500");

                _value = value;
            }
        }

        public TaskPosition(int value)
        {
            this._value = value;
        }

        public static implicit operator TaskPosition(int value)
        {
            return new TaskPosition(value);
        }

        public static implicit operator int(TaskPosition value)
        {
            return value._value;
        }

        /// <summary>
        /// This property is used to prevent the value from being too high, but also to
        /// reorder elements when new elements are inserted or old elements deleted (elements
        /// between x and MaxValue have to be updated either +1 or -1)
        /// </summary>
        public static readonly int MaxValue = 500;
    }
}
