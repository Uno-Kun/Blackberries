namespace Blackberries.ViewModels
{
    public class BuyerRequestForViewingListItemViewModel
    {
        public long Id { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string HousingType { get; set; }

        public long Floor { get; set; }

        public double Area { get; set; }

        public double Price { get; set; }

        public string SellerEmail { get; set; }

        public string SellerName { get; set; }

        public string SellerTelephone { get; set; }

        /// <summary>
        /// Заявка подтвеждена продавцом
        /// </summary>
        public bool SellerConfirmed { get; set; }
    }
}
