namespace TASI.Backend.Domain.Users.Dtos
{
    public class LoginResponseDto
    {
        public UserProfileDto Profile { get; set; }
        public string AccessToken { get; set; }
    }
}
