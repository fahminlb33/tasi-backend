using System;

namespace TASI.Backend.Domain.Manufacture.Dtos
{
    public class ManufactureMaterialDto
    {
        public int MaterialId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
