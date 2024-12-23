using Microsoft.Data.Sqlite;
using Blackberries.ViewModels;

namespace Blackberries.Services
{
    public static class RegistrationService
    {
        public static void Registrate(BuyerViewModel model)
        {
            using (var connection = DataBaseService.GetConnection())
            {
                connection.Open();

                SqliteCommand insert_command = new SqliteCommand();
                insert_command.Connection = connection;
                insert_command.CommandText = $@"INSERT INTO Buyer (email, password, name, telephone)
                    VALUES(@email, @password, @name, @telephone)";

                insert_command.Parameters.AddWithValue("email", model.buyerEmail);
                insert_command.Parameters.AddWithValue("password", model.buyerPassword);
                insert_command.Parameters.AddWithValue("name", model.buyerName);
                insert_command.Parameters.AddWithValue("telephone", model.buyerTelephone);

                insert_command.ExecuteNonQuery();
            }
        }

        public static void Registrate(SellerViewModel model)
        {
            using (var connection = DataBaseService.GetConnection())
            {
                connection.Open();

                SqliteCommand insert_command = new SqliteCommand();
                insert_command.Connection = connection;
                insert_command.CommandText = $@"INSERT INTO Seller (email, password, name, telephone)
                    VALUES(@email, @password, @name, @telephone)";

                insert_command.Parameters.AddWithValue("email", model.sellerEmail);
                insert_command.Parameters.AddWithValue("password", model.sellerPassword);
                insert_command.Parameters.AddWithValue("name", model.sellerName);
                insert_command.Parameters.AddWithValue("telephone", model.sellerTelephone);

                insert_command.ExecuteNonQuery();
            }
        }
    }
}
