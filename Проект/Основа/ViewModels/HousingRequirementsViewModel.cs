namespace Blackberries.ViewModels
{
    public class HousingRequirementsViewModel
    {
        /// <summary>
        /// Идентификатор требований к жилью
        /// null для нового объекта
        /// </summary>
        public long? id { get; set; }

        /// <summary>
        /// Районы городов
        /// </summary>
        public long[]? housingCityDistrict { get; set; }

        /// <summary>
        /// Тип жилья Квартира
        /// </summary>
        public bool? housingType1 { get; set; }

        /// <summary>
        /// Тип жилья Дом
        /// </summary>
        public bool? housingType2 { get; set; }

        /// <summary>
        /// Минимальный этаж
        /// </summary>
        public long? housingMinFloor { get; set; }

        /// <summary>
        /// Минимальная площадь
        /// </summary>
        public double? housingMinArea { get; set; }

        /// <summary>
        /// Максимальная цена
        /// </summary>
        public double? housingMaxPrice { get; set; }
    }
}
