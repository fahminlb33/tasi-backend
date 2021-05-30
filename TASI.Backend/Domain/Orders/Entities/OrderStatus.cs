using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Orders.Entities
{
    public class OrderStatus : IDaoEntity
    {
        [Key]
        public int OrderStatusHistoryId { get; set; }
        public OrderStatusCode Code { get; set; }
        public string Message { get; set; }
        public Order Order { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
