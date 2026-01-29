namespace API.Models.DTOs.Requests.Venue
{
    public class CreateVenueRequest
    {
        public string BusinessName { get; set; }
        public string Location { get; set; }
        public string SubscriptionStatus { get; set; } // "Trial" or "Paid"
    }
}
