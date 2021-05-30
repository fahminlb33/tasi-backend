using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TASI.Backend.Domain.Users.Entities;

namespace TASI.Backend.Domain.Users.Dtos
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal ShippingCost { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserRole Role { get; set; }
    }
}
