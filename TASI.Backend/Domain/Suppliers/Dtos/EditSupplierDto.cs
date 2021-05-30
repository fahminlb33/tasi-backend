#nullable enable

namespace TASI.Backend.Domain.Suppliers.Dtos
{
    public class EditSupplierDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public long? Latitude { get; set; }
        public long? Longitude { get; set; }
    }
}