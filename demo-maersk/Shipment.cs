namespace demo.maersk
{
    public class Shipment
    {
        public string ShipmentNo { get; set; }
        public int ContainersCount { get; set; }
        public string Origin { get; set; }
        public string ImportCountry { get; set; }
        public string CargoType { get; set; }
        public string BookingTimestamp { get; set; }
        public string[] Events { get; set; }
    }
}
