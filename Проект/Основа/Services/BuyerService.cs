namespace Blackberries.Services
{
    using Microsoft.Data.Sqlite;
    using Blackberries.Models;
    using Blackberries.ViewModels;
    using System.Globalization;

    public class BuyerService
    {
        public static void CreateHousingRequirements(HousingRequirementsViewModel model, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand insert_command = new SqliteCommand();
                    insert_command.Connection = connection;
                    insert_command.CommandText = $@"INSERT INTO Housing_requirements (floor, area, max_price, Buyer_id)
                    VALUES(@floor, @area, @max_price, @Buyer_id) RETURNING id";

                    insert_command.Parameters.AddWithValue("floor", model.housingMinFloor == null ? DBNull.Value : model.housingMinFloor);
                    insert_command.Parameters.AddWithValue("area", model.housingMinArea == null ? DBNull.Value : model.housingMinArea);
                    insert_command.Parameters.AddWithValue("max_price", model.housingMaxPrice == null ? DBNull.Value : model.housingMaxPrice);
                    insert_command.Parameters.AddWithValue("Buyer_id", buyerid);

                    var newHousingRequirementsId = insert_command.ExecuteScalar();

                    SqliteCommand update_command = new SqliteCommand();
                    update_command.Connection = connection;
                    update_command.CommandText = $@"UPDATE Buyer SET Housing_requirements_id = @Housing_requirements_id where id = @id";

                    update_command.Parameters.AddWithValue("Housing_requirements_id", newHousingRequirementsId);
                    update_command.Parameters.AddWithValue("id", buyerid);

                    update_command.ExecuteNonQuery();

                    var housingTypes = new List<int>();

                    if (model.housingType1.HasValue && model.housingType1.Value) 
                    {
                        housingTypes.Add(1);
                    }

                    if (model.housingType2.HasValue && model.housingType2.Value)
                    {
                        housingTypes.Add(2);
                    }

                    foreach (int housingType in housingTypes)
                    {
                        SqliteCommand insertHousingType_command = new SqliteCommand();
                        insertHousingType_command.Connection = connection;
                        insertHousingType_command.CommandText = $@"INSERT INTO ""Housing_type/Housing_requirements"" (""Housing_type_id"", ""Housing_requirements_id"")
                                VALUES(@housing_type_id, @housing_requirements_id)";
                        insertHousingType_command.Parameters.AddWithValue("housing_type_id", housingType);
                        insertHousingType_command.Parameters.AddWithValue("housing_requirements_id", newHousingRequirementsId);

                        insertHousingType_command.ExecuteNonQuery();
                    }

                    if (model.housingCityDistrict != null) 
                    {
                        foreach (int cityDistrictId in model.housingCityDistrict)
                        {
                            SqliteCommand insertCityDistrict_command = new SqliteCommand();
                            insertCityDistrict_command.Connection = connection;
                            insertCityDistrict_command.CommandText = $@"INSERT INTO ""A_district_of_the_city/Housing_requirements"" (""A_district_of_the_city_id"", ""Housing_requirements_id"")
                                VALUES(@a_district_of_the_city_id, @housing_requirements_id)";
                            insertCityDistrict_command.Parameters.AddWithValue("a_district_of_the_city_id", cityDistrictId);
                            insertCityDistrict_command.Parameters.AddWithValue("housing_requirements_id", newHousingRequirementsId);

                            insertCityDistrict_command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public static void UpdateHousingRequirements(HousingRequirementsViewModel model, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    var buyer = GetBuyer(buyerid, connection);
                    if (buyer == null)
                    {
                        throw new Exception("Пользователь не определен");
                    }

                    var housingRequirements = GetHousingRequirements(buyer, connection);

                    if (housingRequirements != null)
                    {
                        var oldCityDistricts = GetHousingRequirementsDidtrictOfTheCity(housingRequirements.Id, connection);
                        var oldHousingTypes = GetHousingTypeHousingRequirements(housingRequirements.Id, connection);

                        if (oldCityDistricts.Count > 0) 
                        {
                            SqliteCommand deleteCityDistricts_command = new SqliteCommand();
                            deleteCityDistricts_command.Connection = connection;
                            deleteCityDistricts_command.CommandText = $@"DELETE FROM ""A_district_of_the_city/Housing_requirements"" WHERE Housing_requirements_id = @Housing_requirements_id";
                            deleteCityDistricts_command.Parameters.AddWithValue("Housing_requirements_id", housingRequirements.Id);

                            deleteCityDistricts_command.ExecuteNonQuery();
                        }

                        if (model.housingCityDistrict?.Length > 0) 
                        {
                            foreach (int cityDistrictId in model.housingCityDistrict)
                            {
                                SqliteCommand insertCityDistrict_command = new SqliteCommand();
                                insertCityDistrict_command.Connection = connection;
                                insertCityDistrict_command.CommandText = $@"INSERT INTO ""A_district_of_the_city/Housing_requirements"" (""A_district_of_the_city_id"", ""Housing_requirements_id"")
                                VALUES(@a_district_of_the_city_id, @housing_requirements_id)";
                                insertCityDistrict_command.Parameters.AddWithValue("a_district_of_the_city_id", cityDistrictId);
                                insertCityDistrict_command.Parameters.AddWithValue("housing_requirements_id", housingRequirements.Id);

                                insertCityDistrict_command.ExecuteNonQuery();
                            }
                        }

                        if (oldHousingTypes.Count > 0) 
                        {
                            SqliteCommand deleteHousingTypes_command = new SqliteCommand();
                            deleteHousingTypes_command.Connection = connection;
                            deleteHousingTypes_command.CommandText = $@"DELETE FROM ""Housing_type/Housing_requirements"" WHERE Housing_requirements_id = @Housing_requirements_id";
                            deleteHousingTypes_command.Parameters.AddWithValue("Housing_requirements_id", housingRequirements.Id);

                            deleteHousingTypes_command.ExecuteNonQuery();
                        }

                        var newHousingTypes = new List<int>();

                        if (model.housingType1.HasValue && model.housingType1.Value)
                        {
                            newHousingTypes.Add(1);
                        }

                        if (model.housingType2.HasValue && model.housingType2.Value)
                        {
                            newHousingTypes.Add(2);
                        }

                        foreach (int housingType in newHousingTypes)
                        {
                            SqliteCommand insertHousingType_command = new SqliteCommand();
                            insertHousingType_command.Connection = connection;
                            insertHousingType_command.CommandText = $@"INSERT INTO ""Housing_type/Housing_requirements"" (""Housing_type_id"", ""Housing_requirements_id"")
                                VALUES(@housing_type_id, @housing_requirements_id)";
                            insertHousingType_command.Parameters.AddWithValue("housing_type_id", housingType);
                            insertHousingType_command.Parameters.AddWithValue("housing_requirements_id", housingRequirements.Id);

                            insertHousingType_command.ExecuteNonQuery();
                        }

                        SqliteCommand update_command = new SqliteCommand();
                        update_command.Connection = connection;
                        update_command.CommandText = $@"UPDATE Housing_requirements SET floor = @floor, area = @area, max_price = @max_price where id = @id";

                        update_command.Parameters.AddWithValue("id", housingRequirements.Id);
                        update_command.Parameters.AddWithValue("floor", model.housingMinFloor == null ? DBNull.Value : model.housingMinFloor);
                        update_command.Parameters.AddWithValue("area", model.housingMinArea == null ? DBNull.Value : model.housingMinArea);
                        update_command.Parameters.AddWithValue("max_price", model.housingMaxPrice == null ? DBNull.Value : model.housingMaxPrice);

                        update_command.ExecuteNonQuery();
                    }
                }
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

        public static List<HousingListItemViewModel> GetHousingList(HttpContext context)
        {
            var result = new List<HousingListItemViewModel>();

            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    var buyer = GetBuyer(buyerid, connection);
                    if (buyer == null)
                    {
                        throw new Exception("Пользователь не определен");
                    }

                    var housingRequirements = GetHousingRequirements(buyer, connection);

                    SqliteCommand selectHousing_command = new SqliteCommand();
                    selectHousing_command.Connection = connection;

                    if (housingRequirements == null)
                    {
                        selectHousing_command.CommandText = $@"SELECT h.floor, h.area, h.price, h.Housing_type_id, dc.city, dc.district, h.id from Housing h 
                                                        LEFT JOIN A_district_of_the_city dc ON h.A_district_of_the_city_id = dc.id
                                                        WHERE h.hidden = 0";
                    }
                    else 
                    {
                        var cityDistricts = String.Join(", ", GetHousingRequirementsDidtrictOfTheCity(housingRequirements.Id, connection));
                        var housingTypes = String.Join(", ", GetHousingTypeHousingRequirements(housingRequirements.Id, connection));

                        selectHousing_command.CommandText = $@"SELECT h.floor, h.area, h.price, h.Housing_type_id, dc.city, dc.district, h.id from Housing h 
                                                        LEFT JOIN A_district_of_the_city dc ON h.A_district_of_the_city_id = dc.id
                                                        WHERE h.hidden = 0
                                                        AND (@floor IS NULL OR h.floor >= @floor)
                                                        AND (@area IS NULL OR h.area >= @area)
                                                        AND (@price IS NULL OR h.price <= @price)
                                                        AND (@city_district_id IS NULL OR dc.id IN ({cityDistricts}))
                                                        AND (@housing_type_id IS NULL OR h.Housing_type_id IN ({housingTypes}))";

                        selectHousing_command.Parameters.AddWithValue("floor", housingRequirements.Floor != null ? housingRequirements.Floor: DBNull.Value);
                        selectHousing_command.Parameters.AddWithValue("area", housingRequirements.Area != null ? housingRequirements.Area : DBNull.Value);
                        selectHousing_command.Parameters.AddWithValue("price", housingRequirements.MaxPrice != null ? housingRequirements.MaxPrice : DBNull.Value);
                        selectHousing_command.Parameters.AddWithValue("city_district_id", string.IsNullOrEmpty(cityDistricts) ? DBNull.Value : "" );
                        selectHousing_command.Parameters.AddWithValue("housing_type_id", string.IsNullOrEmpty(housingTypes) ? DBNull.Value : "");
                    }

                    using (SqliteDataReader reader = selectHousing_command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(new HousingListItemViewModel()
                                {
                                    Floor = (long)reader[0],
                                    Area = double.Parse(reader[1].ToString()),
                                    Price = double.Parse(reader[2].ToString()),
                                    HousingType = GetHousingType((long)reader[3]),
                                    City = (string)reader[4],
                                    District = (string)reader[5],
                                    Id = (long)reader[6],
                                });
                            }
                        }
                    }

                    FileService.SetPhotoIds(result, connection);
                }
            }

            return result;
        }

        private static HousingRequirements? GetHousingRequirements(BuyerUser buyer, SqliteConnection connection)
        {
            SqliteCommand selectHousingRequirements_command = new SqliteCommand();
            selectHousingRequirements_command.Connection = connection;
            selectHousingRequirements_command.CommandText = $@"SELECT id, floor, area, max_price from Housing_requirements WHERE Buyer_Id = @buyer";
            selectHousingRequirements_command.Parameters.AddWithValue("buyer", buyer.Id);

            using (SqliteDataReader reader = selectHousingRequirements_command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    return new HousingRequirements(
                        (long)reader[0],
                        reader[1] != DBNull.Value ? (long?)reader[1] : null,
                        reader[2] != DBNull.Value ? double.Parse(reader[2].ToString()) : null,
                        reader[3] != DBNull.Value ? double.Parse(reader[3].ToString()) : null
                        );       
                }
            }

            return null;
        }

        private static List<long> GetHousingRequirementsDidtrictOfTheCity(long housingRequirementsId, SqliteConnection connection)
        {
            var result = new List<long>();

            SqliteCommand selectHousingRequirementsDidtrictOfTheCity_command = new SqliteCommand();
            selectHousingRequirementsDidtrictOfTheCity_command.Connection = connection;
            selectHousingRequirementsDidtrictOfTheCity_command.CommandText = $@"SELECT A_district_of_the_city_id from ""A_district_of_the_city/Housing_requirements"" WHERE Housing_requirements_id = @Housing_requirements_id";
            selectHousingRequirementsDidtrictOfTheCity_command.Parameters.AddWithValue("Housing_requirements_id", housingRequirementsId);

            using (SqliteDataReader reader = selectHousingRequirementsDidtrictOfTheCity_command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add((long)reader[0]);
                    }
                }
            }

            return result;
        }

        private static List<long> GetHousingTypeHousingRequirements(long housingRequirementsId, SqliteConnection connection)
        {
            var result = new List<long>();

            SqliteCommand selectHousingTypeHousingRequirements_command = new SqliteCommand();
            selectHousingTypeHousingRequirements_command.Connection = connection;
            selectHousingTypeHousingRequirements_command.CommandText = $@"SELECT Housing_type_id from ""Housing_type/Housing_requirements"" WHERE Housing_requirements_id = @Housing_requirements_id";
            selectHousingTypeHousingRequirements_command.Parameters.AddWithValue("Housing_requirements_id", housingRequirementsId);

            using (SqliteDataReader reader = selectHousingTypeHousingRequirements_command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add((long)reader[0]);
                    }
                }
            }

            return result;
        }

        private static BuyerUser? GetBuyer(long id, SqliteConnection connection)
        {
            SqliteCommand select_Buyer_command = new SqliteCommand();
            select_Buyer_command.Connection = connection;
            select_Buyer_command.CommandText = "SELECT id, email, name, telephone from Buyer where id=@id";

            select_Buyer_command.Parameters.AddWithValue("id", id);

            using (SqliteDataReader reader = select_Buyer_command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    var tenent_id = (long)reader[0];
                    var tenent_name = (string)reader[2];
                    var tenent_email = (string)reader[1];
                    var tenent_telephone = (string)reader[3];

                    return new BuyerUser(tenent_email, tenent_id, tenent_name, tenent_telephone);
                }
            }
            return null;
        }

        public static void CreateRequestForViewing(long housingId,HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand selectRequestForViewing_command = new SqliteCommand();
                    selectRequestForViewing_command.Connection = connection;
                    selectRequestForViewing_command.CommandText = "SELECT count(*) from Request_for_viewing WHERE Housing_id = @housingId AND Buyer_id = @buyerid";

                    selectRequestForViewing_command.Parameters.AddWithValue("housingId", housingId);
                    selectRequestForViewing_command.Parameters.AddWithValue("buyerid", buyerid);

                    var count = (long)selectRequestForViewing_command.ExecuteScalar();

                    if (count == 0)
                    {
                        SqliteCommand insert_command = new SqliteCommand();
                        insert_command.Connection = connection;
                        insert_command.CommandText = $@"INSERT INTO Request_for_viewing (Buyer_id, Housing_id, seller_confirmed)
                        VALUES(@buyer_id, @housing_id, 0)";

                        insert_command.Parameters.AddWithValue("housing_id", housingId);
                        insert_command.Parameters.AddWithValue("buyer_id", buyerid);

                        insert_command.ExecuteNonQuery();

                        //Отправить уведомление о новой заявке продавцу
                        CreateNewRequestNotification(buyerid, housingId, connection);
                    }
                }
            }
        }

        private static void CreateNewRequestNotification(long buyerid, long housingId, SqliteConnection connection) 
        {
            var buyer = GetBuyer(buyerid, connection);

            SqliteCommand select_command = new SqliteCommand();
            select_command.Connection = connection;
            select_command.CommandText = $@"select cd.city, cd.district, h.Housing_type_id, h.floor, h.area, l.email 
                                            FROM Housing h 
                                            LEFT JOIN Seller l ON h.Seller_id = l.id 
                                            LEFT JOIN A_district_of_the_city cd ON h.A_district_of_the_city_id = cd.id 
                                            WHERE h.id = @housingId";

            select_command.Parameters.AddWithValue("housingId", housingId);

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
                    var sellerEmail = (string)reader[5];
                    var housingDescription = GetHousingDescription(city, district, type, floor, area);
                    var buyerDescription = $"{buyer.Name} , e-mail: {buyer.Email} , phone: {buyer.Telephone}";

                    var subject = "Новая заявка на просмотр жилья";
                    var message = $"Создана новая заявка на просмотр объекта: \n{housingDescription} \nпотенциальным покупателем: {buyerDescription}";

                    EmailNotificationService.Send(subject, message, buyer.Name, sellerEmail);
                }
            }
        }

        private static string GetHousingDescription(string city, string district, string type, long floor, double area)
        {
            return $"{city}, {district}, {type}, {floor} эт., {area} кв.м";
        }

        public static void DeleteRequestForViewing(long requestId, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand deleteRequests_command = new SqliteCommand();
                    deleteRequests_command.Connection = connection;
                    deleteRequests_command.CommandText = $@"DELETE FROM Request_for_viewing WHERE Buyer_id = @buyerId AND id=@requestId";
                    deleteRequests_command.Parameters.AddWithValue("buyerId", buyerid);
                    deleteRequests_command.Parameters.AddWithValue("requestId", requestId);

                    deleteRequests_command.ExecuteNonQuery();
                }
            }
        }

        public static List<BuyerRequestForViewingListItemViewModel> GetBuyerRequestsForViewing(HttpContext context)
        {
            var result = new List<BuyerRequestForViewingListItemViewModel>();

            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();


                    SqliteCommand selectRequests_command = new SqliteCommand();
                    selectRequests_command.Connection = connection;
                    selectRequests_command.CommandText = $@"SELECT r.id, r.seller_confirmed, c.city, c.district, h.floor, h.area, h.price, l.name, l.telephone, l.email, h.Housing_type_id FROM Request_for_viewing r
                                                        LEFT JOIN ((Housing h LEFT JOIN Seller l ON l.id = h.Seller_id) LEFT JOIN A_district_of_the_city c ON c.id = h.A_district_of_the_city_id)
                                                        ON h.id = r.Housing_id
                                                        where r.Buyer_id = @buyerId";
                    selectRequests_command.Parameters.AddWithValue("buyerId", buyerid);

                    using (SqliteDataReader reader = selectRequests_command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(new BuyerRequestForViewingListItemViewModel {
                                    Id = (long)reader[0],
                                    Floor = (long)reader[4],
                                    Area = double.Parse(reader[5].ToString()),
                                    Price = double.Parse(reader[6].ToString()),
                                    City = (string)reader[2],
                                    District = (string)reader[3],
                                    SellerName = (string)reader[7],
                                    SellerEmail = (string)reader[9],
                                    SellerTelephone = (string)reader[8],
                                    SellerConfirmed = (long)reader[1] == 1,
                                    HousingType = GetHousingType((long)reader[10])
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static BuyerViewModel GetCurrentBuyer(HttpContext context) 
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand select_Buyer_command = new SqliteCommand();
                    select_Buyer_command.Connection = connection;
                    select_Buyer_command.CommandText = "SELECT id, email, name, telephone, password from Buyer where id=@id";

                    select_Buyer_command.Parameters.AddWithValue("id", buyerid);

                    using (SqliteDataReader reader = select_Buyer_command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return new BuyerViewModel
                            {
                                id = (long)reader[0],
                                buyerEmail = (string)reader[1],
                                buyerPassword = (string)reader[4],
                                buyerName = (string)reader[2],
                                buyerTelephone = (string)reader[3],
                            };
                        }
                    }
                }
            }

            return null;
        }

        public static void UpdateBuyer(BuyerViewModel model, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                if (buyerid == model.id) //разрешено обновлять только свою анкету
                {
                    using (var connection = DataBaseService.GetConnection())
                    {
                        connection.Open();

                        SqliteCommand update_command = new SqliteCommand();
                        update_command.Connection = connection;
                        update_command.CommandText = $@"UPDATE Buyer SET  
                                                        password = @password, 
                                                        email = @email, 
                                                        name = @name, 
                                                        telephone = @telephone 
                                                        where id = @id";

                        update_command.Parameters.AddWithValue("password", model.buyerPassword);
                        update_command.Parameters.AddWithValue("email", model.buyerEmail);
                        update_command.Parameters.AddWithValue("name", model.buyerName);
                        update_command.Parameters.AddWithValue("telephone", model.buyerTelephone);
                        update_command.Parameters.AddWithValue("id", model.id);

                        update_command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static HousingRequirementsViewModel? GetCurrentBuyerHousingRequirements(HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Buyer)
            {
                var buyerid = userIdentity.Id;

                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    var buyer = GetBuyer(buyerid, connection);
                    if (buyer == null)
                    {
                        throw new Exception("Пользователь не определен");
                    }

                    var housingRequirements = GetHousingRequirements(buyer, connection);

                    if (housingRequirements != null) 
                    {
                        var cityDistricts = GetHousingRequirementsDidtrictOfTheCity(housingRequirements.Id, connection);
                        var housingTypes = GetHousingTypeHousingRequirements(housingRequirements.Id, connection);

                        return new HousingRequirementsViewModel { 
                            id = housingRequirements.Id,
                            housingMinFloor = housingRequirements.Floor,
                            housingMinArea = housingRequirements.Area,
                            housingMaxPrice = housingRequirements.MaxPrice,
                            housingType1 = housingTypes.Contains(1),
                            housingType2 = housingTypes.Contains(2),
                            housingCityDistrict = cityDistricts.ToArray(),
                        };
                    }
                }
            }

            return null;
        }
    }
}
