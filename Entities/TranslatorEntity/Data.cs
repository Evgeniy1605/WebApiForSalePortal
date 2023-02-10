using Newtonsoft.Json;

namespace WebApiForSalePortal.Entities.TranslatorEntity
{
    public class Data
    {
        [JsonProperty("translations")]
        public List<Translation> Translations { get; set; }
    }
}
