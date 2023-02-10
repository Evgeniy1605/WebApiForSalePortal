namespace WebApiForSalePortal.Entities.NovaPoshtaEntities
{
    public class NovaPoshtaInputModel
    {
        public string apiKey { get; set; }
        public string modelName { get; set; }
        public string calledMethod { get; set; }
        public MethodProperties methodProperties { get; set; }
    }
}
