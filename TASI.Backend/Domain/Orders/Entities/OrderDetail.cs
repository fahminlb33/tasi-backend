using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Orders.Entities
{
    public class OrderDetail : IDaoEntity
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public QuantityUnit Unit { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}
