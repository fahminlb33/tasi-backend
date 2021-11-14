using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TASI.Backend.Domain.Manufacture.Entities;

namespace TASI.Backend.Domain.Manufacture.Dtos
{
    public class ManufactureStatusDto
    {
        public int ManufactureStatusId { get; set; }
        public string Message { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ManufactureStatusCode Code { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
