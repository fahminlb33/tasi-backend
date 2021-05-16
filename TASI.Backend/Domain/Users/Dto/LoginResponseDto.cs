namespace TASI.Backend.Domain.Users.Dto
{
    public class LoginResponseDto
    {
        public UserProfileDto Profile { get; set; }
        public string AccessToken { get; set; }
    }
}
