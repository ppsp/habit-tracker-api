using Google.Cloud.Firestore;
using HyperTaskCore.Exceptions;
using HyperTaskCore.Models;
using HyperTaskCore.Services;
using HyperTaskCore.Utils;
using HyperTaskServices.Models.Firestore;
using HyperTaskTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperTaskServices.Services
{
    public class UserService : IUserService
    {
        private FirebaseConnector Connector { get; set; }
        private CalendarTaskService CalendarTaskService { get; set; }

        public UserService(FirebaseConnector connector,
                           CalendarTaskService calendarTaskService)
        {
            this.Connector = connector;
            this.CalendarTaskService = calendarTaskService;
        }

        public async Task<IUser> GetUserAsync(string userId)
        {
            try
            {
                return await getUserAsync(userId);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
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
                    newFireUser.Id = document.Id;
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

        private Query getGetUsersQuery()
        {
            return this.Connector.fireStoreDb
                                 .Collection("user");
        }

        public async Task<bool> InsertUpdateUserAsync(IUser user)
        {
            try
            {
                if (user.LastActivityDate == null)
                    user.LastActivityDate = DateTime.Now.ToUniversalTime();

                if (user.Id != null)
                    return await updateUserAsync(user);
                else
                    return await InsertUserAsync(user);
            }
            catch (Grpc.Core.RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                {
                    return await InsertUserAsync(user);
                }
                else
                {
                    Logger.Error($"GRPC Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task<bool> updateUserAsync(IUser user)
        {
            DocumentReference taskRef = this.Connector.fireStoreDb
                                                      .Collection("user")
                                                      .Document(user.Id);

            var dictionnary = new FireUser(user).ToDictionary();

            await taskRef.UpdateAsync(dictionnary);

            return true;
        }

        public async Task<bool> InsertUserAsync(IUser user)
        {
            try
            {
                await insertUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task insertUserAsync(IUser user)
        {
            CollectionReference colRef = this.Connector.fireStoreDb.Collection("user");
            await colRef.AddAsync(new FireUser(user));
        }

        public async Task<bool> DeleteUserAsync(string Id)
        {
            try
            {
                DocumentReference taskRef = this.Connector.fireStoreDb
                                                          .Collection("user")
                                                          .Document(Id);

                await taskRef.DeleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        public async Task<List<IUser>> GetInactiveAccounts()
        {
            try
            {
                var result = await getInactiveAccounts();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return new List<IUser>();
            }
        }

        private async Task<List<IUser>> getInactiveAccounts()
        {
            // TODO : Get only Activity Date older than one year

            Query query = getGetUsersQuery();

            QuerySnapshot userQuerySnapshot = await query.GetSnapshotAsync();

            List<IUser> users = new List<IUser>();
            foreach (var document in userQuerySnapshot.Documents)
            {
                if (document.Exists)
                {
                    var newFireUser = document.ConvertTo<FireUser>();
                    newFireUser.Id = document.Id;
                    users.Add(newFireUser.ToUser());
                }
            }

            return users;
        }

        public async Task<bool> PermaDeleteUser(IUser user)
        {
            try
            {
                // DELETE FROM USER TABLE
                var result = await DeleteUserWithFireBaseIdAsync(user.Id);

                // DELETE FROM AUTHENTICATION TABLE
                try
                {
                    await this.Connector.DeleteUser(user.UserId);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error deleting from authentication in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                }

                // DELETE FROM CALENDARTASK TABLE
                try
                {
                    var tasks = await CalendarTaskService.GetTasksAsync(user.UserId, true);

                    foreach (var task in tasks)
                    {
                        var result2 = await CalendarTaskService.DeleteTaskWithFireBaseIdAsync(task.CalendarTaskId);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error deleting calendartask table in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        public async Task<bool> DeleteUserWithFireBaseIdAsync(string fireBaseId)
        {
            try
            {
                var firstDocument = this.Connector.fireStoreDb
                                        .Collection("user")
                                        .Document(fireBaseId);

                if (firstDocument != null)
                {
                    await firstDocument.DeleteAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        public async Task UpdateLastActivityDate(string userId, DateTime updateDate)
        {
            var user = await this.GetUserAsync(userId);
            if (updateDate > user.LastActivityDate)
                user.LastActivityDate = updateDate.ToUniversalTime();
            
            await this.InsertUpdateUserAsync(user);
        }
    }
}
