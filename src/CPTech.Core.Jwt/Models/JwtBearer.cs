namespace CPTech.Models
{
    public class JwtBearer
    {
        public string IsEnabled { get; set; }
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }

}
