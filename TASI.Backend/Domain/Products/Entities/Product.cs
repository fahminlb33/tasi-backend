using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Domain.Orders.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Products.Entities
{
    public class Product : IDaoEntity
    {
        [Key]
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }

        public ICollection<Stock> Stocks { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}
