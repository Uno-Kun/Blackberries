namespace Blackberries.ViewModels
{
    public class SellerViewModel
    {
        /// <summary>
        /// Идентификатор анкеты продавца
        /// null для новой анкеты при регистрации
        /// </summary>
        public long? id { get; set; }
        public string sellerEmail { get; set; }
        public string sellerPassword { get; set; }
        public string sellerName { get; set; }
        public string sellerTelephone { get; set; }
    }
}
