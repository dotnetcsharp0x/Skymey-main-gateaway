namespace Skymey_main_Gateway.Models.JWT
{
    public class AuthenticatedResponse
    {
        public string? accessToken { get; set; }
        public string? refreshToken { get; set; }
    }
}
