using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Manufacture.Entities
{
    public class ManufactureMaterial : IDaoEntity
    {
        [Key]
        public int MaterialId { get; set; }

        public ManufactureJob Order { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
