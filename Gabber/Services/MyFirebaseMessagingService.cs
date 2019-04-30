using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Firebase.Messaging;
using Gabber.Activities;

namespace Gabber.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            var notif = message.GetNotification();
            SendNotification(notif.Title, notif.Body, message.Data);
        }

        void SendNotification(string title, string body, IDictionary<string, string> data)
        {
            // The issue here is if we are in a different intent, then it will open the splash activity
            // rather than opening in the current activity ...
            var intent = new Intent(this, typeof(SplashActivity));
            // Do not open new intent if in foreground as it would replace the current one;
            // Only allow notifications with URLs or in background to be clickable.
            if (IsInBackground() || data.ContainsKey("url"))
            {
                if (data.ContainsKey("url"))
                {
                    intent = new Intent(Intent.ActionView);
                    intent.SetData(Uri.Parse(data["url"]));
                }
                else
                {
                    intent.AddFlags(ActivityFlags.BroughtToFront);
                }
            }

            var pendingIntent = PendingIntent.GetActivity(this, 100, intent, PendingIntentFlags.OneShot);
            var notificationBuilder = new NotificationCompat.Builder(this, "NewComment")
                                            .SetSmallIcon(Resource.Drawable.gabber_notif)
                                            .SetContentTitle(title)
                                            .SetColor(ContextCompat.GetColor(ApplicationContext, Resource.Color.colorPrimary))
                                            .SetContentText(body)
                                            .SetAutoCancel(true).SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(100, notificationBuilder.Build());
        }

        public static bool IsInBackground()
        {
            var AppProcessInfo = new ActivityManager.RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(AppProcessInfo);
            return (AppProcessInfo.Importance == Importance.Background || AppProcessInfo.Importance == Importance.Visible);
        }
    }
}
