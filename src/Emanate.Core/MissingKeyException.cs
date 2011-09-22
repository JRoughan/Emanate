using System;

namespace Emanate.Core
{
    public class MissingKeyException : Exception
    {
        public MissingKeyException(string message)
            : base(message) { }
    }
}