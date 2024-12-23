namespace Blackberries.ViewModels
{
    public class BuyerViewModel
    {
        /// <summary>
        /// Идентификатор анкеты покупателя
        /// null для новой анкеты при регистрации
        /// </summary>
        public long? id { get; set; }
        public string buyerEmail { get; set; }
        public string buyerPassword { get; set; }
        public string buyerName { get; set; }
        public string buyerTelephone { get; set; }
    }
}
