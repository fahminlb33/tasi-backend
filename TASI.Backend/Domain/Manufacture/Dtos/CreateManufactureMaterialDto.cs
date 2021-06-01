using System.ComponentModel.DataAnnotations;

namespace TASI.Backend.Domain.Manufacture.Dtos
{
    public class CreateManufactureMaterialDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
