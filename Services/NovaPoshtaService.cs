using Newtonsoft.Json;
using System.Text;
using WebApiForSalePortal.Entities.NovaOoshtaEntities;
using WebApiForSalePortal.Entities.NovaPoshtaEntities;
using WebApiForSalePortal.Entities.TranslatorEntity;

namespace WebApiForSalePortal.Services
{
    public class NovaPoshtaService : INovaPoshtaService
    {
        private readonly string _novaPoshtaUrl;
        private readonly string _novaPoshtaApiKey;
        private readonly IConfiguration _configuration;
        
        public NovaPoshtaService(IConfiguration configuration)
        {
            _configuration = configuration;
            _novaPoshtaUrl = _configuration.GetSection("novaposhtaURL").Value;
            _novaPoshtaApiKey = _configuration.GetSection("novaposhtaApiKey").Value;
            

        }
        public async ValueTask<List<Datum>> GetPostOfficesAsync(string cityName)
        {
            
            List<Datum> PostOffices = new List<Datum>();

            var methodProperties = new MethodProperties();
            methodProperties.CityName = cityName; 
            methodProperties.Limit = "50";

            var root = new NovaPoshtaInputModel();
            root.methodProperties = methodProperties;
            root.modelName = "Address";
            root.calledMethod = "getWarehouses";
            root.apiKey = _novaPoshtaApiKey;

            var postContent = JsonConvert.SerializeObject(root);
            StringContent content = new StringContent(postContent, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_novaPoshtaUrl),
                Content = content


            };
            string resultOfReqest = "";
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                resultOfReqest = body;

            }

            var deserializedResultOfReqest = JsonConvert.DeserializeObject<AddresesOfPostOffice>(resultOfReqest);
            if (deserializedResultOfReqest.success == false)
            {
                return PostOffices;
            }
            PostOffices = deserializedResultOfReqest.data;
            return PostOffices;
        }
    }
}
