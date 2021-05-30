namespace TASI.Backend.Domain.Orders.Entities
{
    public enum OrderStatusCode
    {
        Requested,
        InProcess,
        Delivery,
        Completed,
        Cancelled
    }
}