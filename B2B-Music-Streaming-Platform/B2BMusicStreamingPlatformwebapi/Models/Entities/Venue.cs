namespace API.Models.Entities
{
    public abstract class Venue
    {
        public string VenueID { get; set; }
        public string BusinessName { get; set; }
        public string Location { get; set; }
        public string SubscriptionStatus { get; set; } // "Trial" or "Paid"

        // Abstract method for polymorphic behavior
        public abstract bool CheckStreamLimit(int totalPlaytimeInSeconds);
    }


}
