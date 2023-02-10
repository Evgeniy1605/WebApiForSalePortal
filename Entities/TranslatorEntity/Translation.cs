using Newtonsoft.Json;

namespace WebApiForSalePortal.Entities.TranslatorEntity
{
    public class Translation
    {
        [JsonProperty("translatedText")]
        public string TranslatedText { get; set; }
    }
}
