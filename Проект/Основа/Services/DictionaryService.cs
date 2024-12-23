using Microsoft.Data.Sqlite;
using Blackberries.Models;

namespace Blackberries.Services
{
    public class DictionaryService
    {
        public static List<CityDistrict> GetAllCityDistrict() 
        {
            var result = new List<CityDistrict>();

            using (var connection = DataBaseService.GetConnection())
            {
                connection.Open();

                SqliteCommand select_command = new SqliteCommand();
                select_command.Connection = connection;
                select_command.CommandText = $@"SELECT id, city, district from A_district_of_the_city ORDER BY city ASC, district ASC";

                using (SqliteDataReader reader = select_command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new CityDistrict((long)reader[0], (string)reader[1], (string)reader[2]));
                        }
                    }
                }
            }

            return result;
        }
    }
}
