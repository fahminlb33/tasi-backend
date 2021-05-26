namespace TASI.Backend.Infrastructure.Configs
{
    public class JwtConfig
    {
        public string EncryptionKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
