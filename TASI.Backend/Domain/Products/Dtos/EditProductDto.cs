#nullable enable

using TASI.Backend.Domain.Products.Entities;

namespace TASI.Backend.Domain.Products.Dtos
{
    public class EditProductDto
    {
        public string? Barcode { get; set; }
        public string? Name { get; set; }
        public QuantityUnit? Unit { get; set; }
        public decimal? Price { get; set; }
        public double? Weight { get; set; }
        public bool? CanManufacture { get; set; }
    }
}
