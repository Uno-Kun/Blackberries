using Microsoft.Data.Sqlite;

namespace Blackberries.Services
{
    public static class DataBaseService
    {
        public const string dataBaseName = "C:\\Users\\Дамир\\Desktop\\DB_Blackberries.db";

        public static SqliteConnection GetConnection() 
        { 
            return new SqliteConnection($"Data Source={dataBaseName}");
        }
    }
}
