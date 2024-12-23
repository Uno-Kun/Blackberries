namespace Blackberries.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<HousingListItemViewModel> SellerHousings { get; set; }

        public List<SellerRequestForViewingListItemViewModel> SellerRequestsForViewing { get; set; }

        public List<HousingListItemViewModel> BuyerApplicableHousings { get; set; }

        public List<BuyerRequestForViewingListItemViewModel> BuyerRequestsForViewing { get; set; }

        public List<CityDistrictViewModel> AdminCityDistricts { get; set; }
    }
}
