namespace Sitecore.Feature.PasswordSms.Model
{
    using Newtonsoft.Json;
    using System.Linq;
    
    public partial class ApiResponse
    {
        [JsonProperty("errors")]
        public Error[] Errors { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
        public string ErrorContent
        {
            get
            {
                return
                    Errors == null || !Errors.Any()
                    ? string.Empty
                    : string.Join(",", Errors.Select(f => string.Format("{0}->{1}", f.Code, f.Message)).ToArray());
            }
        }
    }

    public partial class Error
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
