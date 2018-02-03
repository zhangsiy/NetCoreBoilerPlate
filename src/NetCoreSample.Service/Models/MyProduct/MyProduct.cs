using System;

namespace NetCoreSample.Models.MyProduct
{
    public class MyProduct
    {
        public string MyProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyProduct()
        {
        }
    }
}
