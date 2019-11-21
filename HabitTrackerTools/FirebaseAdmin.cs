using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HabitTrackerCore.Exceptions;
using System;
using System.Threading.Tasks;

namespace HabitTrackerTools
{
    public sealed class FirebaseAdmin
    {
        private static readonly Lazy<FirebaseAdmin>
            lazy =
            new Lazy<FirebaseAdmin>
                (() => new FirebaseAdmin());

        public static FirebaseAdmin Instance { get { return lazy.Value; } }

        private string projectId;
        public FirebaseApp firebaseApp;

        private FirebaseAdmin()
        {
            string filepath = Environment.CurrentDirectory + "\\pp-app-1893d-a10a5bc8bf7a.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);

            firebaseApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault()
            });
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
    }
}
