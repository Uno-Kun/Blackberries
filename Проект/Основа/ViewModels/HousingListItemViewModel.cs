namespace Blackberries.ViewModels
{
    public class HousingListItemViewModel
    {
        public long Id { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string HousingType { get; set; }

        public long Floor { get; set; }

        public double Area { get; set; }

        public double Price { get; set; }

        public bool Hidden { get; set; }

        public List<long> PhotoIds { get; set; } = new List<long>();
    }
}
