using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Products.Entities
{
    public class Product : IDaoEntity
    {
        [Key]
        public int ProductId { get; set; }

        public string Barcode { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public double Weight { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public QuantityUnit Unit { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
