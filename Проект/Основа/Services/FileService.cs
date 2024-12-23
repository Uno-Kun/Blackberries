using Microsoft.Data.Sqlite;
using Blackberries.Models;
using Blackberries.ViewModels;

namespace Blackberries.Services
{
    public static class FileService
    {
        public static void RemoveAll(long housingId, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand delete_command = new SqliteCommand();
                    delete_command.Connection = connection;
                    delete_command.CommandText = $@"DELETE FROM HousingPhoto WHERE Housing_id = @Housing_id";

                    delete_command.Parameters.AddWithValue("Housing_id", housingId);

                    delete_command.ExecuteNonQuery();
                }
            }
        }

        public static void UploadFile(long housingId, IFormFile file, HttpContext context)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);

            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity && userIdentity.Role == UserRole.Seller)
            {
                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand insert_command = new SqliteCommand();
                    insert_command.Connection = connection;
                    insert_command.CommandText = $@"INSERT INTO HousingPhoto (file_name, file_extension, content, Housing_id, content_type)
                    VALUES(@file_name, @file_extension, zeroblob(@length), @Housing_id, @content_type) RETURNING id";

                    insert_command.Parameters.AddWithValue("file_name", fileName);
                    insert_command.Parameters.AddWithValue("file_extension", fileExtension);
                    insert_command.Parameters.AddWithValue("length", file.Length);
                    insert_command.Parameters.AddWithValue("Housing_id", housingId);
                    insert_command.Parameters.AddWithValue("content_type", file.ContentType);

                    var photoId = (long)insert_command.ExecuteScalar();

                    using (var writeStream = new SqliteBlob(connection, "HousingPhoto", "content", photoId))
                    {
                        file.CopyTo(writeStream);
                    }
                }
            }
        }

        public static (MemoryStream stream, string contentType)? GetFile(long fileId, HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity != null && identity.IsAuthenticated && identity is UserIdentity userIdentity)
            {
                using (var connection = DataBaseService.GetConnection())
                {
                    connection.Open();

                    SqliteCommand select_command = new SqliteCommand();
                    select_command.Connection = connection;
                    select_command.CommandText = $@"SELECT content_type, content FROM HousingPhoto WHERE id=@id";

                    select_command.Parameters.AddWithValue("id", fileId);

                    using (var reader = select_command.ExecuteReader())
                    {
                        if (reader.HasRows) 
                        {
                            reader.Read();

                            var contentType = (string)reader[0];
                            var stream = new MemoryStream();

                            using (var readStream = reader.GetStream(1))
                            {
                                readStream.CopyTo(stream);
                            }

                            return (stream, contentType);
                        }
                    }
                }
            }

            return null;
        }

        public static void SetPhotoIds(List<HousingListItemViewModel> housingList, SqliteConnection connection)
        {
            var housingDict = housingList.ToDictionary(x => x.Id, x => x);
            var ids = string.Join(", ", housingList.Select(x => x.Id));

            SqliteCommand select_command = new SqliteCommand();
            select_command.Connection = connection;
            select_command.CommandText = $@"SELECT id, Housing_id FROM HousingPhoto WHERE Housing_id in ({ids})";

            using (SqliteDataReader reader = select_command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = (long)reader[0];
                        var housingId = (long)reader[1];

                        housingDict[housingId].PhotoIds.Add(id);
                    }
                }
            }
        }
    }
}
