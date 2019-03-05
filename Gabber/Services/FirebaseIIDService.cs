
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Firebase.Iid;

namespace Gabber.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            SendRegistrationToServer(refreshedToken);

            Console.WriteLine(FirebaseInstanceId.Instance.Token);
        }

        private void SendRegistrationToServer(string token)
        {
            // associate the user's registration token with the server-side 
            // account (if any) that is maintained by the application
        }
    }
}
