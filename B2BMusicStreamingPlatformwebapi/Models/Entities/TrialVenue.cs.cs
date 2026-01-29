namespace API.Models.Entities
{
    public class TrialVenue : Venue
    {
        public override bool CheckStreamLimit(int totalPlaytimeInSeconds)
        {
            return totalPlaytimeInSeconds <= 1800; // 30 minutes
        }
    }
}
