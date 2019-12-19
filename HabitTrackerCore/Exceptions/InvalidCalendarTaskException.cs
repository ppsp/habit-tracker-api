using System;
using System.Runtime.Serialization;

namespace HabitTrackerCore.Exceptions
{
    [Serializable]
    public class InvalidCalendarTaskException : Exception
    {
        public InvalidCalendarTaskException()
        {
        }

        public InvalidCalendarTaskException(string message) : base(message)
        {
        }

        public InvalidCalendarTaskException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidCalendarTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}