using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyWebService.Controllers.Api.Common
{
    /// <summary>
    /// A builder to construct ETags
    /// </summary>
    internal class ETagBuilder
    {
        /// <summary>
        /// List of tokens to utilize for the e-tag construction
        /// </summary>
        private List<object> Tokens { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ETagBuilder()
        {
            Tokens = new List<object>();
        }

        /// <summary>
        /// Take in the specified token into e-tag construction 
        /// </summary>
        public ETagBuilder WithToken(object token)
        {
            Tokens.Add(token);
            return this;
        }

        /// <summary>
        /// Build the e-tag from all the specified tokens
        /// </summary>
        /// <returns>The produced e-tag</returns>
        public string Build()
        {
            var str = string.Join("-", Tokens);
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(str))).Replace("-", string.Empty);
        }
    }
}
