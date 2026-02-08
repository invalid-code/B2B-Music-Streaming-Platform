namespace API.Models.DTOs.Requests.Auth
{
    /// <summary>
    /// Request model for user registration (creating a new venue).
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Email address of the business owner.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password for the account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Full name of the business owner.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Name of the venue/business.
        /// </summary>
        public string VenueName { get; set; }

        /// <summary>
        /// Physical location of the venue.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// User role: "BusinessOwner" or "Staff"
        /// </summary>
        public string Role { get; set; } = "BusinessOwner";

        /// <summary>
        /// Business registration number (optional).
        /// </summary>
        public string BusinessRegistrationNumber { get; set; }
    }
}
