using System;
using System.ComponentModel.DataAnnotations;

namespace NetCoreSample.Models.DeveloperSample
{
    public class MyProduct
    {
        [StringLength(100)]
        public string MyProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime Modified { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyProduct()
        {
        }
    }
}
