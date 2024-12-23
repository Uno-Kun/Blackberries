namespace Blackberries.Services
{
    using Microsoft.Data.Sqlite;
    using Blackberries.Models;
    using Blackberries.ViewModels;
    using System.Globalization;

    public static class SellerService
    {
        public static SellerViewModel GetCurrentSeller(HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                var sellerId = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand select_Seller_command = new SqliteCommand();
                    select_Seller_command.Connection = connection;
                    select_Seller_command.CommandText = "SELECT id, email, name, telephone, password from Seller where id=@id";

                    select_Seller_command.Parameters.AddWithValue("id", sellerId);

                    using (SqliteDataReader reader = select_Seller_command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return new SellerViewModel {
                                id = (long)reader[0],
                                sellerPassword = (string)reader[4],
                                sellerEmail = (string)reader[1],
                                sellerName = (string)reader[2],
                                sellerTelephone = (string)reader[3],
                            };
                        }
                    }
                }
            }

            return null;
        }

        public static void UpdateSeller(SellerViewModel model, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                var sellerId = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand update_Seller_command = new SqliteCommand();
                    update_Seller_command.Connection = connection;
                    update_Seller_command.CommandText = @"UPDATE Seller SET 
                                                                email = @email,
                                                                name = @name,
                                                                telephone = @telephone,
                                                                password = @password
                                                            WHERE id=@id";

                    update_Seller_command.Parameters.AddWithValue("id", sellerId);
                    update_Seller_command.Parameters.AddWithValue("email", model.sellerEmail);
                    update_Seller_command.Parameters.AddWithValue("name", model.sellerName);
                    update_Seller_command.Parameters.AddWithValue("telephone", model.sellerTelephone);
                    update_Seller_command.Parameters.AddWithValue("password", model.sellerPassword);

                    update_Seller_command.ExecuteNonQuery();
                }
            }
        }

        private static long GetHousingTypeId(string housingTypeName)
        {
            switch (housingTypeName)
            {
                case "Квартира": return 1;
                case "Дом": return 2;
                default: throw new Exception("Unreachable code");
            }
        }

        private static string GetHousingType(long housingTypeId) 
        {
            switch (housingTypeId)
            {
                case 1: return "Квартира";
                case 2: return "Дом";
                default: throw new Exception("Unreachable code");
            }
        }

        public static HousingViewModel GetHousing(long id, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                var sellerId = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand select_command = new SqliteCommand();
                    select_command.Connection = connection;
                    select_command.CommandText = @"SELECT h.id, h.floor, h.area, h.price, ht.type, h.A_district_of_the_city_id, h.hidden  from Housing h 
                                                   LEFT JOIN Housing_type ht ON h.Housing_type_id = ht.id 
                                                   WHERE h.id = @id AND h.Seller_id=@seller_id";

                    select_command.Parameters.AddWithValue("id", id);
                    select_command.Parameters.AddWithValue("seller_id", sellerId);

                    using (SqliteDataReader reader = select_command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return new HousingViewModel { 
                                id = (long)reader[0],
                                housingFloor = (long)reader[1],
                                housingArea = double.Parse(reader[2].ToString()),
                                housingPrice = double.Parse(reader[3].ToString()),
                                housingType = (string)reader[4],
                                housingCityDistrict = (long)reader[5],
                                hidden = reader[6] == DBNull.Value ? false : (long)reader[6] == 1,
                            };
                        }
                    }
                }
            }

            return null;
        }

        public static void CreateHousing(HousingViewModel model, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                var sellerId = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand insert_command = new SqliteCommand();
                    insert_command.Connection = connection;
                    insert_command.CommandText = $@"INSERT INTO Housing (floor, area, price, Housing_type_id, Seller_id, A_district_of_the_city_id, hidden)
                    VALUES(@floor, @area, @price, @Housing_type_id, @Seller, @A_district_of_the_city_id, @Hidden)";

                    insert_command.Parameters.AddWithValue("floor", model.housingFloor);
                    insert_command.Parameters.AddWithValue("area", model.housingArea);
                    insert_command.Parameters.AddWithValue("price", model.housingPrice);
                    insert_command.Parameters.AddWithValue("Housing_type_id", GetHousingTypeId(model.housingType));
                    insert_command.Parameters.AddWithValue("Seller", sellerId);
                    insert_command.Parameters.AddWithValue("A_district_of_the_city_id", model.housingCityDistrict);
                    insert_command.Parameters.AddWithValue("Hidden", model.hidden);

                    insert_command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateHousing(HousingViewModel model, HttpContext context) 
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                var sellerId = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand update_Housing_command = new SqliteCommand();
                    update_Housing_command.Connection = connection;
                    update_Housing_command.CommandText = $@"UPDATE Housing SET 
                                                        floor = @floor, 
                                                        area = @area, 
                                                        price = @price,
                                                        Housing_type_id = @Housing_type_id, 
                                                        Seller_id = @Seller, 
                                                        A_district_of_the_city_id = @A_district_of_the_city_id,
                                                        hidden = @hidden
                                                    WHERE id = @id";

                    update_Housing_command.Parameters.AddWithValue("id", model.id);
                    update_Housing_command.Parameters.AddWithValue("floor", model.housingFloor);
                    update_Housing_command.Parameters.AddWithValue("area", model.housingArea);
                    update_Housing_command.Parameters.AddWithValue("price", model.housingPrice);
                    update_Housing_command.Parameters.AddWithValue("Housing_type_id", GetHousingTypeId(model.housingType));
                    update_Housing_command.Parameters.AddWithValue("Seller", sellerId);
                    update_Housing_command.Parameters.AddWithValue("A_district_of_the_city_id", model.housingCityDistrict);
                    update_Housing_command.Parameters.AddWithValue("hidden", model.hidden);

                    update_Housing_command.ExecuteNonQuery();
                }
            }
        }

        public static List<HousingListItemViewModel> GetHousingList(HttpContext context)
        {
            var result = new List<HousingListItemViewModel>();

            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                var sellerId = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();
                    SqliteCommand select_command = new SqliteCommand();
                    select_command.Connection = connection;
                    select_command.CommandText = $@"SELECT h.floor, h.area, h.price, h.Housing_type_id, dc.city, dc.district, h.id, h.hidden from Housing h LEFT JOIN A_district_of_the_city dc ON h.A_district_of_the_city_id = dc.id WHERE h.Seller_id=@Seller";
                    select_command.Parameters.AddWithValue("Seller", sellerId);

                    using (SqliteDataReader reader = select_command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read()) 
                            {
                                var floor = (long)reader[0];
                                var area = double.Parse(reader[1].ToString());
                                var price = double.Parse(reader[2].ToString());
                                var housingTypeId = (long)reader[3];
                                var city = reader[4] == DBNull.Value ? null: (string?)reader[4];
                                var district = reader[5] == DBNull.Value ? null : (string?)reader[5];
                                var id = (long)reader[6];
                                var hidden = (long)reader[7] == 1;

                                result.Add(new HousingListItemViewModel 
                                { 
                                    Id = id,
                                    City = city,
                                    District = district,
                                    Area = area,
                                    Price = price,
                                    Floor = floor,
                                    HousingType = GetHousingType(housingTypeId),
                                    Hidden = hidden,
                                });
                            }
                        }
                    }

                    FileService.SetPhotoIds(result, connection);
                }
            }

            return result;
        }

        public static List<SellerRequestForViewingListItemViewModel> GetRequestList(HttpContext context) 
        {
            var result = new List<SellerRequestForViewingListItemViewModel>();

            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                var sellerId = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();
                    SqliteCommand select_command = new SqliteCommand();
                    select_command.Connection = connection;
                    select_command.CommandText = $@"SELECT r.id, r.seller_confirmed, 
                                                            dc.city, dc.district, 
                                                            ht.type, 
                                                            h.floor, h.area, 
                                                            t.email, t.name, t.telephone
                                                    FROM Request_for_viewing r 
                                                    LEFT JOIN Housing h ON r.Housing_id = h.id 
                                                    LEFT JOIN Buyer t ON t.id = r.Buyer_id
                                                    LEFT JOIN A_district_of_the_city dc ON dc.id = h.A_district_of_the_city_id
                                                    LEFT JOIN Housing_type ht ON ht.id = h.Housing_type_id
                                                    WHERE h.Seller_id=@Seller";

                    select_command.Parameters.AddWithValue("Seller", sellerId);

                    using (SqliteDataReader reader = select_command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(new SellerRequestForViewingListItemViewModel { 
                                    id = (long)reader[0],
                                    sellerConfirmed = (long)reader[1] == 1,
                                    housingDescription = GetHousingDescription(
                                        (string)reader[2],
                                        (string)reader[3],
                                        (string)reader[4],
                                        (long)reader[5],
                                        double.Parse(reader[6].ToString())),
                                    buyerEmail = (string)reader[7],
                                    buyerName = (string)reader[8],
                                    buyerTelephone = (string)reader[9],
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static string GetHousingDescription(HousingListItemViewModel housing) 
        {
            return GetHousingDescription(housing.City, housing.District, housing.HousingType, housing.Floor, housing.Area);
        }

        private static string GetHousingDescription(string city, string district, string type, long floor, double area)
        {
            return $"{city}, {district}, {type}, {floor} эт., {area} кв.м";
        }

        public static void SetRequestConfirmation(long requestId, bool value, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand update_command = new SqliteCommand();
                    update_command.Connection = connection;
                    update_command.CommandText = $@"UPDATE Request_for_viewing SET seller_confirmed = @seller_confirmed WHERE id = @id";

                    update_command.Parameters.AddWithValue("id", requestId);
                    update_command.Parameters.AddWithValue("seller_confirmed", value);

                    update_command.ExecuteNonQuery();

                    if (value) 
                    {
                        //Отправить уведомление покупателю о подтверждении заявки
                        CreateRequestConfirmationNotification(requestId, connection);
                    }
                }
            }
        }

        private static void CreateRequestConfirmationNotification(long requestId, SqliteConnection connection)
        {
            SqliteCommand select_command = new SqliteCommand();
            select_command.Connection = connection;
            select_command.CommandText = $@"select cd.city, cd.district, h.Housing_type_id, h.floor, h.area, 
                                            t.email as buyer_email, 
                                            l.name as seller_name , l.email as seller_email, l.telephone as seller_phone
                                            FROM Request_for_viewing r
                                            LEFT JOIN Housing h ON r.Housing_id = h.id
                                            LEFT JOIN Seller l ON h.Seller_id = l.id 
                                            LEFT JOIN A_district_of_the_city cd ON h.A_district_of_the_city_id = cd.id 
                                            LEFT JOIN Buyer t ON r.Buyer_id = t.id
                                            WHERE r.id = @requestId AND r.seller_confirmed = 1";

            select_command.Parameters.AddWithValue("requestId", requestId);

            using (SqliteDataReader reader = select_command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    var city = (string)reader[0];
                    var district = (string)reader[1];
                    var type = GetHousingType((long)reader[2]);
                    var floor = (long)reader[3];
                    var area = double.Parse(reader[4].ToString());
                    var buyerEmail = (string)reader[5];
                    var sellerName = (string)reader[6];
                    var sellerEmal = (string)reader[7];
                    var sellerPhone = (string)reader[8];
                    var housingDescription = GetHousingDescription(city, district, type, floor, area);
                    var sellerDescription = $"{sellerName} , e-mail: {sellerEmal} , phone: {sellerPhone}";

                    var subject = "Подтверждена заявка на просмотр жилья";
                    var message = $"Подтверждена заявка на просмотр объекта: \n{housingDescription} \nпотенциальным продавцом: {sellerDescription}";

                    EmailNotificationService.Send(subject, message, sellerName, buyerEmail);
                }
            }
        }
    }
}
