using Google.Cloud.Firestore;
using System;

namespace HabitTrackerTools
{
    public sealed class FirestoreConnector
    {
        private static readonly Lazy<FirestoreConnector>
            lazy =
            new Lazy<FirestoreConnector>
                (() => new FirestoreConnector());

        public static FirestoreConnector Instance { get { return lazy.Value; } }

        private string projectId;
        public FirestoreDb fireStoreDb;

        private FirestoreConnector()
        {
            string filepath = Environment.CurrentDirectory + "\\pp-app-1893d-a10a5bc8bf7a.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "pp-app-1893d";
            fireStoreDb = FirestoreDb.Create(projectId);
        }
    }
}
