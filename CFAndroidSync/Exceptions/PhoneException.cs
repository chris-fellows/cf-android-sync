using System.Globalization;

namespace CFAndroidSync.Exceptions
{
    /// <summary>
    /// General phone exception
    /// </summary>
    public class PhoneException : Exception
    {        
        public PhoneException()
        {
        }

        public PhoneException(string message) : base(message)
        {
        }

        public PhoneException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PhoneException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
