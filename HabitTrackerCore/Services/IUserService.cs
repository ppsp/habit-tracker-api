using HabitTrackerCore.Models;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface IUserService
    {
        Task<bool> UpdateUserAsync(IUser user);
        Task<IUser> GetUserAsync(string userId);
    }
}
