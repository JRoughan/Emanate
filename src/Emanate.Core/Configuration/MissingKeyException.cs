using System;

namespace Emanate.Core.Configuration
{
    public class MissingKeyException : Exception
    {
        public MissingKeyException(string message)
            : base(message) { }
    }
}