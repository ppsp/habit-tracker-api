using Google.Cloud.Firestore;
using HabitTrackerCore.Exceptions;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerCore.Utils;
using HabitTrackerServices.Models.Firestore;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabitTrackerServices.Services
{
    public class UserService : IUserService
    {
        private FirebaseConnector Connector { get; set; }

        public UserService(FirebaseConnector connector)
        {
            this.Connector = connector;
        }

        public async Task<IUser> GetUserAsync(string userId)
        {
            try
            {
                return await getUserAsync(userId);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetUserAsync", ex);
                return NULLUser.Instance;
            }
        }

        private async Task<IUser> getUserAsync(string userId)
        {
            Query query = getGetUserQuery(userId);

            QuerySnapshot userQuerySnapshot = await query.GetSnapshotAsync();

            foreach (var document in userQuerySnapshot.Documents)
            {
                if (document.Exists)
                {
                    var newFireUser = document.ConvertTo<FireUser>();
                    return newFireUser.ToUser();
                }
            }

            return NULLUser.Instance;
        }

        private Query getGetUserQuery(string userId)
        {
            return this.Connector.fireStoreDb
                                 .Collection("user")
                                 .WhereEqualTo("UserId", userId);
        }

        public async Task<bool> UpdateUserAsync(IUser user)
        {
            try
            {
                return await updateUserAsync(user);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in UpdateTaskAsyncNoPositionCheck", ex);
                return false;
            }
        }

        private async Task<bool> updateUserAsync(IUser user)
        {
            DocumentReference taskRef = this.Connector.fireStoreDb
                                                      .Collection("user")
                                                      .Document(user.Id);

            var dictionnary = user.ToDictionary();

            await taskRef.UpdateAsync(dictionnary);

            return true;
        }


        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                DocumentReference taskRef = this.Connector.fireStoreDb
                                                          .Collection("user")
                                                          .Document(userId);

                await taskRef.DeleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Error in DeleteUserAsync", ex);
                return false;
            }
        }
    }
}
