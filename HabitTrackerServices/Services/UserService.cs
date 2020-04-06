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

        public async Task<bool> InsertUpdateUserAsync(IUser user)
        {
            try
            {
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
                    Logger.Error("GRPC Error in InsertUpdateUserAsync", ex);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error in InsertUpdateUserAsync", ex);
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
                Logger.Error("Error in InsertUserAsync", ex);
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
                Logger.Error("Error in DeleteUserAsync", ex);
                return false;
            }
        }
    }
}
