using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TASI.Backend.Domain.Orders.Entities;

namespace TASI.Backend.Domain.Orders.Dtos
{
    public class OrderStatusDto
    {
        public int OrderStatusHistoryId { get; set; }
        public string Message { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatusCode Code { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
