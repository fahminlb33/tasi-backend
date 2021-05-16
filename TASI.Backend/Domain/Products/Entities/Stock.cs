using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Domain.Orders.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Products.Entities
{
    public class Stock : IDaoEntity
    {
        [Key]
        public int StockId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public QuantityUnit Unit { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}
