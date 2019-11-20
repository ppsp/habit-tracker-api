using System;
using System.Runtime.Serialization;

namespace HabitTrackerCore.Exceptions
{
    [Serializable]
    public class InvalidJwtTokenException : Exception
    {
        public InvalidJwtTokenException()
        {
        }

        public InvalidJwtTokenException(string message) : base(message)
        {
        }

        public InvalidJwtTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidJwtTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}