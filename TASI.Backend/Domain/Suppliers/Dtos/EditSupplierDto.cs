#nullable enable

namespace TASI.Backend.Domain.Suppliers.Dtos
{
    public class EditSupplierDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}