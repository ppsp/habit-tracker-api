using System;
using System.Runtime.Serialization;

namespace HyperTaskCore.Exceptions
{
    [Serializable]
    public class TaskPositionInvalidException : Exception
    {
        public TaskPositionInvalidException()
        {
        }

        public TaskPositionInvalidException(string message) : base(message)
        {
        }

        public TaskPositionInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TaskPositionInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}