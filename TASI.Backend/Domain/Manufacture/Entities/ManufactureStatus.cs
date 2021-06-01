using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Manufacture.Entities
{
    public class ManufactureStatus : IDaoEntity
    {
        [Key]
        public int ManufactureStatusId { get; set; }

        public ManufactureStatusCode Code { get; set; }
        public string Message { get; set; }
        public ManufactureJob Order { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
