using System.ComponentModel.DataAnnotations;

namespace Skymey_main_Gateway.Models.Tables.User
{
    public class SU_001R
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string refreshToken { get; set; }
        public DateTime refreshTokenExpiryTime { get; set; }
    }
}