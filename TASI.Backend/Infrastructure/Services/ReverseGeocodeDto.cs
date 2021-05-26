namespace TASI.Backend.Infrastructure.Services
{
    public record ReverseGeocodeDto(bool Success, string Address, string GeocodedAddress, double Latitude, double Longitude);
}