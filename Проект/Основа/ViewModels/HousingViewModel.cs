namespace Blackberries.ViewModels
{
    public class HousingViewModel
    {
        /// <summary>
        /// Идентификатор жилья 
        /// null для нового объекта
        /// </summary>
        public long? id { get; set; }
        public long housingCityDistrict { get; set; }
        public string housingType { get; set; }
        public long housingFloor { get; set; }
        public double housingArea { get; set; }
        public double housingPrice { get; set; }
        public bool hidden { get; set; }
    }
}
