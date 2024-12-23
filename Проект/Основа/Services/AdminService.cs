using Microsoft.Data.Sqlite;
using Blackberries.Models;
using Blackberries.ViewModels;

namespace Blackberries.Services
{
    public static class AdminService
    {
        public static List<CityDistrictViewModel> GetCityDistricts(HttpContext context)
        {
            var result = new List<CityDistrictViewModel>();

            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Admin)
            {
                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand select_command = new SqliteCommand();
                    select_command.Connection = connection;

                    select_command.CommandText = $@"SELECT id, city, district from A_district_of_the_city";

                    using (SqliteDataReader reader = select_command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(new CityDistrictViewModel { 
                                    Id = (long)reader[0],
                                    City = (string)reader[1],
                                    District = (string)reader[2],
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static void CreateCityDistrict(CityDistrictViewModel model, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Admin)
            {
                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand create_command = new SqliteCommand();
                    create_command.Connection = connection;

                    create_command.CommandText = $@"INSERT INTO A_district_of_the_city (city, district) VALUES (@city, @district)";
                    create_command.Parameters.AddWithValue("city", model.City);
                    create_command.Parameters.AddWithValue("district", model.District);

                    create_command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateCityDistrict(CityDistrictViewModel model, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Admin)
            {
                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand update_command = new SqliteCommand();
                    update_command.Connection = connection;

                    update_command.CommandText = $@"UPDATE A_district_of_the_city SET city = @city, district = @district WHERE id = @id";
                    update_command.Parameters.AddWithValue("id", model.Id);
                    update_command.Parameters.AddWithValue("city", model.City);
                    update_command.Parameters.AddWithValue("district", model.District);

                    update_command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteCityDistrict(long cityDistrictId, HttpContext context) 
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Admin)
            {
                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand delete_command = new SqliteCommand();
                    delete_command.Connection = connection;

                    delete_command.CommandText = $@"DELETE FROM A_district_of_the_city WHERE id = @id";
                    delete_command.Parameters.AddWithValue("id", cityDistrictId);

                    delete_command.ExecuteNonQuery();
                }
            }
        }
    }
}
