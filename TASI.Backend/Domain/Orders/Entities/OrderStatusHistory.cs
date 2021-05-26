using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Orders.Entities
{
    public class OrderStatusHistory : IDaoEntity
    {
        [Key]
        public int OrderStatusHistoryId { get; set; }
        public OrderStatus Code { get; set; }
        public string Message { get; set; }
        public Order Order { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
