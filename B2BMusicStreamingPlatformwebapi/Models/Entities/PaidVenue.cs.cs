namespace API.Models.Entities
{
    public class PaidVenue : Venue
    {
        public override bool CheckStreamLimit(int totalPlaytimeInSeconds)
        {
            return true; // Always allow playback
        }
    }

}
