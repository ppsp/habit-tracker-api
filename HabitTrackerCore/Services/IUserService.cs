using HabitTrackerCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface IUserService
    {
        Task<bool> InsertUpdateUserAsync(IUser user);
        Task<IUser> GetUserAsync(string userId);
        Task<List<IUser>> GetInactiveAccounts();
        Task<bool> PermaDeleteUser(IUser user);
        Task UpdateLastActivityDate(string userId);
    }
}
