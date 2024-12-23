namespace Blackberries.Models
{
    /// <summary>
    /// Район города
    /// </summary>
    public class CityDistrict
    {
        private long id;
        private string city;
        private string district;

        public CityDistrict(long id, string city, string district)
        {
            this.id = id;
            this.city = city;
            this.district = district;
        }

        public long Id => this.id;

        public string City => this.city;

        public string District => this.district;
    }
}