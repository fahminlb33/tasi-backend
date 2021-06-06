using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TASI.Backend.Domain.Orders.Entities;
using TASI.Backend.Domain.Suppliers.Entities;
using TASI.Backend.Domain.Users.Dtos;

namespace TASI.Backend.Domain.Orders.Dtos
{
    public class SimpleOrderDto
    {
        public int OrderId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType Type { get; set; }

        public double TotalWeight { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal TotalTax { get; set; }
        public decimal SubTotal { get; set; }
        public OrderStatusDto LastStatus { get; set; }
        public UserProfileDto PicUser { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
