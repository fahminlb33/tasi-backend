using System.ComponentModel.DataAnnotations;

namespace TASI.Backend.Domain.Orders.Dtos
{
    public class OrderCommandProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
