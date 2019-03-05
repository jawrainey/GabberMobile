using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Common;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using Firebase;
using Firebase.Analytics;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Models;
using Newtonsoft.Json;
using Gabber.Services;
using System;

namespace Gabber.Activities
{
    [Activity(Label = "Gabber", Theme = "@style/SplashTheme", MainLauncher = true)]
    public class SplashActivity : Activity, Application.IActivityLifecycleCallbacks
    {
        private static NotificationManager notificationManager;
        public string PlayServicesErrorMessage;
        private bool? PlayServicesAvailable;

        private void EnsureSessionIsValid()
        {
            if (Session.PrivatePath == null)
            {
                Session.PrivatePath = new PrivatePath();
            }

            if (RestClient.GlobalIO == null)
            {
                RestClient.GlobalIO = new DiskIO();
            }

            if (!Fabric.Fabric.IsInitialized)
            {
                Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
                Crashlytics.Crashlytics.HandleManagedExceptions();
            }

            if (MainActivity.FireBaseAnalytics == null)
            {
                FirebaseApp.InitializeApp(ApplicationContext);
                MainActivity.FireBaseAnalytics = FirebaseAnalytics.GetInstance(this);
            }

            PlayServicesAvailable = IsPlayServicesAvailable();

            if (notificationManager == null)
            {
                CreateNotificationChannel();
            }
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    PlayServicesErrorMessage = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }

                else
                {
                    PlayServicesErrorMessage = "This device is not supported by Google Play Services";
                }

                return false;
            }
            return true;
        }

#pragma warning disable XA0001 // Find issues with Android API usage
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            notificationManager = (NotificationManager)GetSystemService(NotificationService);

            notificationManager.CreateNotificationChannel(new NotificationChannel("TalkFutures_UploadReminder",
                                                  "Upload Queue Reminders",
                                                   NotificationImportance.Low)
            {

                Description = "Reminds you to upload forgotten items in your uploads queue."
            });

            notificationManager.CreateNotificationChannel(new NotificationChannel("TalkFutures_NoActivity",
                                                  "Reminders to Contribute",
                                                  NotificationImportance.Low)
            {

                Description = "Reminds you to contribute to the TalkFutures project."
            });

            notificationManager.CreateNotificationChannel(new NotificationChannel("TalkFutures_NewContent",
                                                  "New Discussion Topics",
                                                  NotificationImportance.Default)
            {

                Description = "Notifies you when new topics become available."
            });
        }
#pragma warning restore XA0001 // Find issues with Android API usage

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {

        }

        public void OnActivityDestroyed(Activity activity)
        {

        }

        public void OnActivityPaused(Activity activity)
        {

        }

        public void OnActivityResumed(Activity activity)
        {
            EnsureSessionIsValid();
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {

        }

        public void OnActivityStarted(Activity activity)
        {

        }

        public void OnActivityStopped(Activity activity)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Localise.SetLayoutDirectionByCulture(this);

            Application.RegisterActivityLifecycleCallbacks(this);

            CheckAuth();
        }

        private void CheckAuth()
        {
            EnsureSessionIsValid();

            var preferences = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var UserEmail = preferences.GetString("username", "");

            if (string.IsNullOrWhiteSpace(UserEmail))
            {
                var onboardingIntent = new Intent(this, typeof(Onboarding));
                onboardingIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
                StartActivity(onboardingIntent);
            }
            else
            {
                var user = Queries.UserByEmail(UserEmail);
                var tokens = JsonConvert.DeserializeObject<JWToken>(preferences.GetString("tokens", ""));
                Queries.SetActiveUser(new DataUserTokens { User = user, Tokens = tokens });
                MainActivity.FireBaseAnalytics.SetUserId(Session.ActiveUser.Id.ToString());

                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            Finish();
        }
    }
}
