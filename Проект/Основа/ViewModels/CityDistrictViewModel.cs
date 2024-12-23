namespace Blackberries.ViewModels
{
    public class CityDistrictViewModel
    {
        /// <summary>
        /// Идентификатор записи  справочника
        /// null для новой записи справочника
        /// </summary>
        public long? Id { get; set; }

        public string City { get; set; }

        public string District { get; set; }
    }
}
