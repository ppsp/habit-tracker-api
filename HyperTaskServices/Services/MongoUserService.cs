using HyperTaskCore.Models;
using HyperTaskCore.Services;
using HyperTaskServices.Models.Mongo;
using HyperTaskTools;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperTaskServices.Services
{
    public class MongoUserService : IUserService
    {
        private readonly string DBHyperTask = "hypertask";
        private const string CollectionUser = "user";
        private FirebaseConnector _FirebaseConnector { get; set; }
        private MongoConnector Connector { get; set; }
        private MongoCalendarTaskService CalendarTaskService { get; set; }
        private MongoTaskGroupService TaskGroupService { get; set; }

        public MongoUserService(MongoConnector connector,
                                MongoCalendarTaskService calendarTaskService,
                                MongoTaskGroupService taskGroupService,
                                FirebaseConnector firebaseConnector)
        {
            this.Connector = connector;
            this.CalendarTaskService = calendarTaskService;
            this.TaskGroupService = taskGroupService;
            this._FirebaseConnector = firebaseConnector;
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
            var userQuery = await getGetUserQuery(userId);
            var users = userQuery.ToList();

            if (users.Count > 0)
                return users[0].ToUser();

            return NULLUser.Instance;
        }

        private async Task<IAsyncCursor<MongoUser>> getGetUserQuery(string userId)
        {
            var filter = Builders<MongoUser>.Filter.Eq(p => p.UserId, userId);
            var userQuery = await this.Connector.mongoClient
                                                .GetDatabase(DBHyperTask)
                                                .GetCollection<MongoUser>(CollectionUser)
                                                .FindAsync<MongoUser>(filter);

            return userQuery;
        }

        private async Task<IAsyncCursor<MongoUser>> getGetUsersQuery()
        {
            var userQuery = await this.Connector.mongoClient
                                                .GetDatabase(DBHyperTask)
                                                .GetCollection<MongoUser>(CollectionUser)
                                                .FindAsync(Builders<MongoUser>.Filter.Empty);

            return userQuery;
        }

        public async Task<bool> InsertUpdateUserAsync(IUser user)
        {
            try
            {
                if (user.InsertDate == DateTime.MinValue)
                    user.InsertDate = DateTime.Now.ToUniversalTime();

                return await InsertUserAsync(user);
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
                var userQuery = await getGetUserQuery(userId);
                var users = userQuery.ToList();

                try
                {
                    return users.Count > 0;
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

        public async Task<bool> InsertUserAsync(IUser user)
        {
            try
            {
                await replaceUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task replaceUserAsync(IUser user)
        {
            var mongoUser = new MongoUser(user);
            user.Id = null;
            var filter = Builders<MongoUser>.Filter.Eq(p => p.UserId, user.UserId);
            var result = await this.Connector.mongoClient
                                             .GetDatabase(DBHyperTask)
                                             .GetCollection<MongoUser>(CollectionUser)
                                             .ReplaceOneAsync(filter, mongoUser, new ReplaceOptions() { IsUpsert = true });

            user.Id = mongoUser.Id;
        }

        public async Task<bool> DeleteUserAsync(string Id)
        {
            try
            {
                var filter = Builders<MongoUser>.Filter.Eq(p => p.Id, Id);
                var result = await this.Connector.mongoClient
                                                 .GetDatabase(DBHyperTask)
                                                 .GetCollection<MongoUser>(CollectionUser)
                                                 .DeleteOneAsync(filter);

                return result.DeletedCount == 1;
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

            var userQuery = await getGetUsersQuery();
            var users = userQuery.ToList();

            List<IUser> usersResult = new List<IUser>();
            foreach (var document in users)
            {
                if (document != null)
                {
                    usersResult.Add(document.ToUser());
                }
            }

            return usersResult;
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
                    await this._FirebaseConnector.DeleteUser(user.UserId);
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
                var filter = Builders<MongoUser>.Filter.Eq(p => p.Id, fireBaseId);
                var result = await this.Connector.mongoClient
                                                 .GetDatabase(DBHyperTask)
                                                 .GetCollection<MongoUser>(CollectionUser)
                                                 .DeleteOneAsync(filter);

                return result.DeletedCount == 1;
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

            return await this._FirebaseConnector.ValidateUserId(userId, jwt);
        }
    }
}
