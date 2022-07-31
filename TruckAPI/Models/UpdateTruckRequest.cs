namespace TruckAPI.Models
{
    public class UpdateTruckRequest
    {
        public string TruckName { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
