using Newtonsoft.Json;

namespace WebApiForSalePortal.Entities.TranslatorEntity
{
    public class TranslationRoot
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
