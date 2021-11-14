using System;
using System.Collections.Generic;
using TASI.Backend.Domain.Products.Entities;

namespace TASI.Backend.Domain.Manufacture.Dtos
{
    public class ManufactureJobDto
    {
        public int ManufactureId { get; set; }
        public Product Product { get; set; }
        public int ExpectedProduce { get; set; }
        public DateTime ExpectedCompletion { get; set; }
        public int FinalProduce { get; set; }

        public IEnumerable<ManufactureMaterialDto> Materials { get; set; }
        public IEnumerable<ManufactureStatusDto> StatusHistory { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
