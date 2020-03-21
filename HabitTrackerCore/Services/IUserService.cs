using HabitTrackerCore.Models;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface IUserService
    {
        Task<bool> InsertUpdateUserAsync(IUser user);
        Task<IUser> GetUserAsync(string userId);
    }
}
