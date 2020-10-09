using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using HyperTaskCore.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace HyperTaskTools
{
    public sealed class FirebaseConnector
    {
        public FirestoreDb fireStoreDb;
        public FirebaseApp firebaseApp;

        public FirebaseConnector(string firebaseSecretJson)
        {
            try
            {
                JObject json = JObject.Parse(firebaseSecretJson);

                GoogleCredential googleCreds = GoogleCredential.FromJson(firebaseSecretJson);
                Channel channel = new Channel(FirestoreClient.DefaultEndpoint.Host,
                                              FirestoreClient.DefaultEndpoint.Port,
                                              googleCreds.ToChannelCredentials());
                FirestoreClient client = FirestoreClient.Create(channel);

                var projectId = json["project_id"].ToString();
                fireStoreDb = FirestoreDb.Create(projectId, client);

                firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = googleCreds
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to create FirestoreConnector", ex);
            }
        }

        public async Task<bool> ValidateJwt(string jwt)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                                                               .VerifyIdTokenAsync(jwt);

                string uid = decodedToken.Uid;

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Error in ValidateJwt", ex);
                throw new InvalidJwtTokenException("Error in ValidateJwt", ex);
            }
        }

        public async Task<bool> DeleteUser(string userId)
        {
            try
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(userId);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Error in ValidateJwt", ex);
                throw new InvalidJwtTokenException("Error in ValidateJwt", ex);
            }
        }
    }
}
