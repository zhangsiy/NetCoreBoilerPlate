using System;

namespace NetCoreSample.Data.Exceptions
{
    /// <summary>
    /// Exception where one attempts at creating an object into the repository, 
    /// but another object with the same key already exists.
    /// </summary>
    internal class AlreadyExistsException : Exception
    {
        internal AlreadyExistsException(string message) :
            base(message)
        {

        }

        internal AlreadyExistsException(string message, Exception innerException) :
            base(message, innerException)
        {

        }
    }
}
