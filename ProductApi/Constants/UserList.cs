using ProductApi.Models;

namespace ProductApi.Constants
{
    public class UserList
    {
        public static List<User> Users = new List<User>()
        {
            new User() { Username = "jperez", Password = "admin123", Rol = "Admin", Email = "camm@gmail.com", FirstName = "Juan"},
            new User() { Username = "mgonzalez", Password = "admin123", Rol = "SalesMan", Email = "mgonzalez@gmail.com", FirstName = "Maria"},
        };
    }
}
