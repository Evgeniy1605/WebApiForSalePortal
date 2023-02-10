using WebApiForSalePortal.Entities.TranslatorEntity;

namespace WebApiForSalePortal.Services
{
    public interface ITranslator
    {
        public ValueTask<string> TranslateAsync(string targetLanguage, string sourceLanguage, string content);
        
    }
}
