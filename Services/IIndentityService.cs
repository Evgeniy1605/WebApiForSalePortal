using WebApiForSalePortal.Models;

namespace WebApiForSalePortal.Services
{
    public interface IIndentityService
    {
        public ValueTask<UserOutPutModel> ValidateUserAsync(string userName, string password);

    }
}
