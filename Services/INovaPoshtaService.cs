using WebApiForSalePortal.Entities.NovaOoshtaEntities;

namespace WebApiForSalePortal.Services
{
    public interface INovaPoshtaService
    {
        public ValueTask<List<Datum>> GetPostOfficesAsync(string cityName);
    }
}
