using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Manufacture.Entities
{
    public class ManufactureJob : IDaoEntity
    {
        [Key]
        public int ManufactureId { get; set; }
        public Product Product { get; set; }
        public int ExpectedProduce { get; set; }
        public DateTime ExpectedCompletion { get; set; }
        public int FinalProduce { get; set; }

        public ICollection<ManufactureMaterial> Materials { get; set; }
        public ICollection<ManufactureStatus> StatusHistory { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}
