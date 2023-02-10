using Newtonsoft.Json;

namespace WebApiForSalePortal.Entities.NovaOoshtaEntities
{
    public class Info
    {
        [JsonProperty("totalCount")]
        public int totalCount { get; set; }
    }
}
