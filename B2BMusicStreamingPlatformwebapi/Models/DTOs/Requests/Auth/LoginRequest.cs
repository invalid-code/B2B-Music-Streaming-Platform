namespace API.Models.DTOs.Requests.Auth
{
    /// <summary>
    /// Request model for user login.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; }
    }
}
