namespace TASI.Backend.Domain.Maps.Dtos
{
    public class LookupAddressDto
    {
        public string OriginalAddress { get; set; }
        public string GeocodedAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
