using Newtonsoft.Json;

namespace PassionStore.Application.DTOs.Identities
{
    public class GoogleLoginResponse
    {
        [JsonProperty("sub")]
        public string Sub { get; set; } = string.Empty;
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("given_name")]
        public string GivenName { get; set; } = string.Empty;
        [JsonProperty("family_name")]
        public string FamilyName { get; set; } = string.Empty;
        [JsonProperty("picture")]
        public string Picture { get; set; } = string.Empty;
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; } = false;
    }
}
