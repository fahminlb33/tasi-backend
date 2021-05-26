#nullable enable

using TASI.Backend.Domain.Users.Entities;

namespace TASI.Backend.Domain.Users.Dtos
{
    public class EditUserDto
    {
        public string? FullName { get; set; }
        public UserRole? Role { get; set; }
        public string? Username { get; set; }
        public string? Address { get; set; }
        public long? Latitude { get; set; }
        public long? Longitude { get; set; }
        public decimal? ShippingCost { get; set; }
    }
}