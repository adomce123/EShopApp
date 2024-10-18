namespace EshopApiGateway.Settings
{
    public class AuthSettings
    {
        public required string Authority { get; set; } = string.Empty;
        public required string Audience { get; set; } = string.Empty;
        public required string ValidIssuer { get; set; } = string.Empty;
        public string[] ValidAudiences { get; set; } = Array.Empty<string>();
        public bool ValidateLifetime { get; set; } = true;
        public bool ValidateIssuerSigningKey { get; set; } = true;
        public int ClockSkew { get; set; } = 0;
    }
}
