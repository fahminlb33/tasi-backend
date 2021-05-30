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
        public OrderType Type { get; set; }
        public double TotalWeight { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal TotalTax { get; set; }
        public decimal SubTotal { get; set; }
        public Supplier Supplier { get; set; }
        public User PicUser { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<OrderStatus> StatusHistory { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}
