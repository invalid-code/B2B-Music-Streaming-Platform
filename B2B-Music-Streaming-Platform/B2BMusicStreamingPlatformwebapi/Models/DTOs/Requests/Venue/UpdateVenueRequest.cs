namespace API.Models.DTOs.Requests.Venue
{
    public class UpdateVenueRequest
    {
        public string VenueID { get; set; }
        public string BusinessName { get; set; }
        public string Location { get; set; }
        public string SubscriptionStatus { get; set; }
    }
}
