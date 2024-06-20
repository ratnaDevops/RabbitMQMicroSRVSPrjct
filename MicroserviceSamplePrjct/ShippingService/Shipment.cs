namespace ShippingService
{
    public class Shipment
    {
        public int ShipmentId { get; set; }
        public int OrderId { get; set; }
        public string TrackingId { get; set; }
    }
}
