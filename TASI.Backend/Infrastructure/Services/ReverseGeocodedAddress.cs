namespace TASI.Backend.Infrastructure.Services
{
    public record ReverseGeocodedAddress(bool Success, string Address, string GeocodedAddress, double Latitude, double Longitude);
}