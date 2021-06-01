using System;

namespace TASI.Backend.Domain.Manufacture.Dtos
{
    public class SimpleManufactureJobDto
    {
        public int ManufactureId { get; set; }
        public int ProductId { get; set; }
        public int ExpectedProduce { get; set; }
        public DateTime ExpectedCompletion { get; set; }
        public int FinalProduce { get; set; }

        public ManufactureStatusDto LastStatus { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
