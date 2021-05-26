namespace TASI.Backend.Domain.Maps.Dtos
{
    public record ShippingCostDto(
        double SourceLatitude, 
        double SourceLongitude, 
        double TotalDistance,
        decimal ShippingCost);
}
