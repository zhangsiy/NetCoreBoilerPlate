using System;

namespace NetCoreSample.Service.Common.Repository
{
    internal class NotFoundException : System.Exception
    {
        internal NotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {

        }
    }
}