namespace TASI.Backend.Domain.Orders.Entities
{
    public enum OrderStatus
    {
        Requested,
        InProcess,
        Delivery,
        Completed,
        Cancelled
    }
}