using System.ComponentModel.DataAnnotations;

namespace TASI.Backend.Domain.Orders.Dto
{
    public class OrderCommandProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
