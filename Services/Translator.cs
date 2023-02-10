using Newtonsoft.Json;
using WebApiForSalePortal.Entities.TranslatorEntity;

namespace WebApiForSalePortal.Services
{
    public class Translator : ITranslator
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Translator> _logger;
        private readonly string _key;
        private readonly string _host;
        private readonly string _url;
        public Translator(IConfiguration configuration, ILogger<Translator> logger)
        {
            _configuration= configuration;
            _logger= logger;
            _key = _configuration.GetSection("Translator:X-RapidAPI-Key").Value;
            _host = _configuration.GetSection("Translator:X-RapidAPI-Host").Value;
            _url = _configuration.GetSection("Translator:Url").Value;

        }
        public async ValueTask<string> TranslateAsync(string targetLanguage, string sourceLanguage, string content)
        {
            string body = "";
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_url),
                    Headers =
                    {
                        { "X-RapidAPI-Key", _key },
                        { "X-RapidAPI-Host", _host },
                    },
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "q", content },
                        { "target", targetLanguage },
                        { "source", sourceLanguage },
                    }),
                };
                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    body = await response.Content.ReadAsStringAsync();

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                finally
                {
                    request.Dispose();
                }
            }
            var deserializedClass = JsonConvert.DeserializeObject<TranslationRoot>(body);

            return deserializedClass.Data.Translations.First().TranslatedText;
            
        }
    }
}
