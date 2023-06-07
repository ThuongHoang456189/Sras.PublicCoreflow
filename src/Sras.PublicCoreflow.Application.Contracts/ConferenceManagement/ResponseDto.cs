using Newtonsoft.Json;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ResponseDto
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
