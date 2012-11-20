using System;

namespace CustomerManagement
{
    [global::System.Serializable]
    public class CustomerServiceException : Exception
    {
        public enum ErrorCode
        {
            UnexpectedException,
            CustomerNotFound
        }

        public CustomerServiceException(ErrorCode e) : base(e.ToString()) { }
        public CustomerServiceException(ErrorCode e, string message) : base(e.ToString() + ": " + message) { }
        public CustomerServiceException(ErrorCode e, string message, Exception inner) : base(e.ToString() + ": " + message, inner) { }
        protected CustomerServiceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
