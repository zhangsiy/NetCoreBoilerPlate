using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreSample.Service.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Take entries from one disctionary, and use them to set values in 
        /// the source dictionary.
        /// If a key is found in the source, the value would be updated
        /// If a key is not found in the source, a new entry would be created in source.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newValues"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> SetValues<TKey, TValue>(
            this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> newValues)
        {
            if (newValues == null)
            {
                return original;
            }

            if (original == null)
            {
                return new Dictionary<TKey, TValue>(newValues);
            }

            IDictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(original);
            newValues.ToList().ForEach(keyValue => result[keyValue.Key] = keyValue.Value);
            return result;
        }
    }
}
