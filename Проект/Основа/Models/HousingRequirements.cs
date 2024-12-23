namespace Blackberries.Models
{
    /// <summary>
    /// требования к жилью
    /// </summary>
    public class HousingRequirements
    {
        private long id;
        private long? floor;
        private double? area;
        private double? maxPrice;

        public HousingRequirements(long id, long? floor, double? area, double? maxPrice)
        {
            this.id = id;
            this.floor = floor;
            this.area = area;
            this.maxPrice = maxPrice;
        }

        public long Id => this.id;
        public long? Floor => this.floor;
        public double? Area => this.area;
        public double? MaxPrice => this.maxPrice;
    }
}
