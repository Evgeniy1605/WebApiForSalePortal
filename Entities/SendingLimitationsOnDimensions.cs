using Newtonsoft.Json;

namespace WebApiForSalePortal.Entities
{
    public class SendingLimitationsOnDimensions
    {
        [JsonProperty("Width")]
        public int Width { get; set; }

        [JsonProperty("Height")]
        public int Height { get; set; }

        [JsonProperty("Length")]
        public int Length { get; set; }
    }
}
