using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Domain.Suppliers.Entities;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Orders.Entities
{
    public class Order : IDaoEntity
    {
        [Key]
        public int OrderId { get; set; }
        public OrderStatus StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int PicUserId { get; set; }
        public User PicUser { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}
