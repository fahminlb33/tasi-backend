using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TASI.Backend.Domain.Products.Entities;

namespace TASI.Backend.Domain.Orders.Dto
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public double TotalWeight { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public QuantityUnit Unit { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}
