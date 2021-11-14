using System.ComponentModel.DataAnnotations;
using TASI.Backend.Domain.Manufacture.Entities;

namespace TASI.Backend.Domain.Manufacture.Dtos
{
    public class ProcessManufactureDto
    {
        [Required]
        public ManufactureStatusCode Code { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Message { get; set; }

        public int? FinalProduce { get; set; }
    }
}
