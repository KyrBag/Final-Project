using TaskManagerApp.DTOs;
using TaskManagerApp.Models;

namespace TaskManagerApp.Services
{
    public interface IUserService
    {
        
        string GenerateUserToken(string userId, string? userName, string? email,
            string? appSecurityKey, IList<string> roles);
      
    }
}
