using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Blackberries.Models;
using System.Globalization;

namespace Blackberries.Services
{
    public static class AuthenticationService
    {
        public static AuthenticationResult Authenticate(string login, string password) 
        {
            var adminUserConfig = ServiceProvider.Instance.GetRequiredService<IOptions<AdminUserConfig>>().Value;

            if (adminUserConfig.Email == login && adminUserConfig.Password == password) 
            {
                return new AuthenticationResult
                {
                    Success = true,
                    AppUser = new AdminUser(adminUserConfig.Email, adminUserConfig.Name),
                };
            }

            using (var connection = DataBaseService.GetConnection()) 
            {
                connection.Open();

                SqliteCommand select_Seller_command = new SqliteCommand();
                select_Seller_command.Connection = connection;
                select_Seller_command.CommandText = "SELECT id, telephone, email, name from Seller where email=@login and password=@password";

                select_Seller_command.Parameters.AddWithValue("login", login);
                select_Seller_command.Parameters.AddWithValue("password", password);

                using (SqliteDataReader reader = select_Seller_command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        var seller_id = (long)reader[0];
                        var seller_telephone = (string)reader[1];
                        var seller_email = (string)reader[2];
                        var seller_name = (string)reader[3];

                        return new AuthenticationResult
                        {
                            Success = true,
                            AppUser = new SellerUser(seller_email, seller_id, seller_telephone, seller_name),
                        };
                    }
                }

                SqliteCommand select_Buyer_command = new SqliteCommand();
                select_Buyer_command.Connection = connection;
                select_Buyer_command.CommandText = "SELECT id, name, email, telephone from Buyer where email=@login and password=@password";

                select_Buyer_command.Parameters.AddWithValue("login", login);
                select_Buyer_command.Parameters.AddWithValue("password", password);

                using (SqliteDataReader reader = select_Buyer_command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        var tenent_id = (long)reader[0];
                        var tenent_name = (string)reader[1];
                        var tenent_email = (string)reader[2];
                        var tenent_telephone = (string)reader[3];

                        return new AuthenticationResult
                        {
                            Success = true,
                            AppUser = new BuyerUser(tenent_email, tenent_id, tenent_name, tenent_telephone),
                        };
                    }
                }
            }

            return new AuthenticationResult
            {
                Success = false,
                AppUser = null,
            };
        }
    }
}
