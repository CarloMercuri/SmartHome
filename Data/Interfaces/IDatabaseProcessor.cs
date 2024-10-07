using SmartHome.Data.Users;

namespace SmartHome.Data.Interfaces
{
    public interface IDatabaseProcessor
    {
        bool CreateUserData(UserData _data);
        UserData GetUserData(string userEmail);
        UserData GetUserDataFromToken(string refreshToken);
        bool UpdateRefreshToken(string userEmail, string refreshToken);
    }
}
