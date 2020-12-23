using HyperTaskCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyperTaskCore.Services
{
    public interface IUserService
    {
        Task<bool> InsertUpdateUserAsync(IUser user);
        Task<IUser> GetUserAsync(string userId);
        Task<List<IUser>> GetAllUsersAsync();
        Task<bool> PermaDeleteUser(IUser user);
        Task UpdateLastActivityDate(string userId, DateTime updateDate);
        Task<bool> ValidateUserId(string userId, string jwt);
    }
}
