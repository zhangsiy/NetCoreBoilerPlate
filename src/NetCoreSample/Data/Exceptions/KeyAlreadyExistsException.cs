using System;

namespace NetCoreSample.Data.Exceptions
{
    /// <summary>
    /// Exception where one attempts at creating a new Candidate Dynamic Design, 
    /// but one already exists with the same ID.
    /// </summary>
    public class KeyAlreadyExistsException : Exception
    {
        public KeyAlreadyExistsException()
            : base("The item being added has a key that already exists in the database.")
        {
        }
    }
}
