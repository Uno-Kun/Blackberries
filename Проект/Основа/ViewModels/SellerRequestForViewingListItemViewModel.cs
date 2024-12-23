namespace Blackberries.ViewModels
{
    public class SellerRequestForViewingListItemViewModel
    {
        public long id { get; set; }
        public string housingDescription { get; set; }
        public string buyerEmail { get; set; }
        public string buyerName { get; set; }
        public string buyerTelephone { get; set; }

        /// <summary>
        /// Заявка подтвеждена продавцом
        /// </summary>
        public bool sellerConfirmed { get; set; }
    }
}
