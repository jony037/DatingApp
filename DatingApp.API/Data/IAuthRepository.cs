using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user,String password);
         Task<User> Login(string name,String password);
          Task<bool> UserExists(string name);

    }
}