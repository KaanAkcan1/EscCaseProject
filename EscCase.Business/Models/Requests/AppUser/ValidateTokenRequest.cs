namespace EscCase.Business.Models.Requests.AppUser
{
    public class ValidateTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty ;
    }
}
