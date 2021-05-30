using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TASI.Backend.Domain.Orders.Entities;

namespace TASI.Backend.Domain.Orders.Dto
{
    public class OrderStatusHistoryDto
    {
        public int OrderStatusHistoryId { get; set; }
        public string Message { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus Code { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
