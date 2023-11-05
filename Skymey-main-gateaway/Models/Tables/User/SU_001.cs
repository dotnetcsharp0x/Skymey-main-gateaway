using System.ComponentModel.DataAnnotations;

namespace Skymey_main_Gateway.Models.Tables.User
{
    public class SU_001
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}