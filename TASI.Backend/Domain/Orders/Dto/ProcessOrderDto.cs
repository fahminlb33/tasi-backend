using System.ComponentModel.DataAnnotations;
using TASI.Backend.Domain.Orders.Entities;

namespace TASI.Backend.Domain.Orders.Dto
{
    public class ProcessOrderDto
    {
        [Required]
        public OrderStatusCode Code { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Message { get; set; }
    }
}