using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Suppliers.Entities
{
    public class Supplier : IDaoEntity
    {
        [Key]
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal ShippingCost { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}
