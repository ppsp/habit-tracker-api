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
    public class FireUserService : IUserService
    {
        private const string table_name = "user";
        private FirebaseConnector Connector { get; set; }
        private FireCalendarTaskService CalendarTaskService { get; set; }
        private FireTaskGroupService TaskGroupService { get; set; }

        public FireUserService(FirebaseConnector connector,
                           FireCalendarTaskService calendarTaskService,
                           FireTaskGroupService taskGroupService)
        {
            this.Connector = connector;
            this.CalendarTaskService = calendarTaskService;
            this.TaskGroupService = taskGroupService;
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
                                 .Collection(table_name)
                                 .WhereEqualTo("UserId", userId);
        }

        private Query getGetUsersQuery()
        {
            return this.Connector.fireStoreDb
                                 .Collection(table_name);
        }

        public async Task<bool> InsertUpdateUserAsync(IUser user)
        {
            try
            {
                if (user.InsertDate == DateTime.MinValue)
                    user.InsertDate = DateTime.Now.ToUniversalTime();

                bool alreadyExists = await CheckIfExistsAsync(user.UserId);
                if (alreadyExists)
                {
                    return await updateUserAsync(user);
                }
                else
                {
                    return await InsertUserAsync(user);
                }
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

        private async Task<bool> CheckIfExistsAsync(string userId)
        {
            try
            {
                Query query = this.Connector.fireStoreDb
                                            .Collection(table_name)
                                            .WhereEqualTo("UserId", userId);

                try
                {
                    QuerySnapshot tasksQuerySnapshot = await query.GetSnapshotAsync();

                    foreach (var document in tasksQuerySnapshot.Documents)
                    {
                        if (document.Exists)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                throw ex;
            }
        }

        private async Task<bool> updateUserAsync(IUser user)
        {
            Query query = this.Connector.fireStoreDb
                                        .Collection(table_name)
                                        .WhereEqualTo("UserId", user.UserId);

            var allDocuments = (await query.GetSnapshotAsync()).Documents;

            // Should not occur but just in case, we delete duplicate ids
            if (allDocuments.Count > 1)
            {
                await deleteDuplicates(allDocuments, user);
                allDocuments = new List<DocumentSnapshot>() { allDocuments[0] };
            }

            var firstDocument = allDocuments.SingleOrDefault();

            if (firstDocument != null && firstDocument.Exists)
            {
                var dictionnary = new FireUser(user).ToDictionary();

                await firstDocument.Reference.UpdateAsync(dictionnary);

                return true;
            }

            return false;
        }

        private async Task deleteDuplicates(IReadOnlyList<DocumentSnapshot> allDocuments, IUser user)
        {
            Logger.Warn("DUPLICATE DOCUMENT WHEN UPDATING, DELETING EXTRA, UserId" + user.UserId);

            var toDeletes = allDocuments.OrderBy(p => p.CreateTime).Skip(1);

            foreach (var toDelete in toDeletes)
            {
                await DeleteUserWithFireBaseIdAsync(toDelete.Id);
            }
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
            CollectionReference colRef = this.Connector.fireStoreDb.Collection(table_name);
            await colRef.AddAsync(new FireUser(user));
        }

        public async Task<bool> DeleteUserAsync(string Id)
        {
            try
            {
                DocumentReference taskRef = this.Connector.fireStoreDb
                                                          .Collection(table_name)
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

        public async Task<List<IUser>> GetAllUsersAsync()
        {
            try
            {
                var result = await getAllUsersAsync();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return new List<IUser>();
            }
        }

        private async Task<List<IUser>> getAllUsersAsync()
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

                // DELETE FROM GROUP TABLE
                try
                {
                    var groups = await TaskGroupService.GetGroupsAsync(user.UserId, true);

                    foreach (var group in groups)
                    {
                        var result2 = await TaskGroupService.DeleteGroupWithFireBaseIdAsync(group.Id);
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
                                        .Collection(table_name)
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

        public async Task<bool> ValidateUserId(string userId, string jwt)
        {
            if (jwt == null) // For tests we don't use jwt
                return true;

            return await this.Connector.ValidateUserId(userId, jwt);
        }
    }
}
