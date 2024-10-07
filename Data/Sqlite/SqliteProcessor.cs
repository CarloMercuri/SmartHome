using SmartHome.Data.Interfaces;
using SmartHome.Data.Tokens;
using SmartHome.Data.Users;
using SQLite;
using System.Runtime.CompilerServices;

namespace SmartHome.Data.Sqlite
{
    public class SqliteProcessor : IDatabaseProcessor
    {
        private string _dbName = "sh_db";
        private string _dbPath = "";

        public SqliteProcessor()
        {
            _dbPath = $"localdata\\SQLite\\{_dbName}.db";
        }

        public UserData GetUserData(string userEmail)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
            {
                if (conn.GetTableInfo("UserData").Count <= 0)
                {
                    conn.CreateTable<UserData>();
                }
                var query = conn.Table<UserData>().FirstOrDefault(x => x.UserEmail == userEmail);

                return query;
            }
        }

        public bool CreateUserData(UserData _data)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
            {
                if (conn.GetTableInfo("UserData").Count <= 0)
                {
                    conn.CreateTable<UserData>();
                }

                int result = conn.Insert(_data);

                return result > 0;
            }
        }

        public bool UpdateRefreshToken(string userEmail, string refreshToken)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
                {

                    var sql = "UPDATE UserData SET RefreshToken = @RefreshToken WHERE UserEmail = @Username";
                    conn.Execute(sql, new
                    {
                        RefreshToken = refreshToken,
                        UserEmail = userEmail
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           
            
        }

        public UserData GetUserDataFromToken(string refreshToken)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
            {
                if (conn.GetTableInfo("RefreshTokenData").Count <= 0)
                {
                    conn.CreateTable<RefreshTokenData>();
                }
                RefreshTokenData data = conn.Table<RefreshTokenData>().FirstOrDefault(x => x.Token == refreshToken);

                if (data != null) 
                {
                    return GetUserData(data.UserEmail);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
