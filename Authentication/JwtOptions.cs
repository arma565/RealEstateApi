namespace RealEstate.Authentication
{
    #pragma warning disable CA1515
    public class JwtOptions
    {
        public string Secret { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }
}
