using SQLite;

namespace SmartHome.Data.Users
{
    public class UserData
    {
        [PrimaryKey]
        public string UserEmail { get; set; }   
        public string UserPasswordHash { get; set; }
        public string UserSalt {  get; set; }
    }
}
