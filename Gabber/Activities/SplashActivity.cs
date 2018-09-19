﻿using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Firebase;
using Firebase.Analytics;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Models;
using Newtonsoft.Json;

namespace Gabber.Activities
{
    [Activity(Label = "TalkFutures", Theme = "@style/SplashTheme", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Localise.SetLayoutDirectionByCulture(this);

            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
            Crashlytics.Crashlytics.HandleManagedExceptions();

            Init();
        }

        private void Init()
        {
            FirebaseApp.InitializeApp(ApplicationContext);
            MainActivity.FireBaseAnalytics = FirebaseAnalytics.GetInstance(this);

            // Used by the PCL for database interactions so must be defined early.
            Session.PrivatePath = new PrivatePath();
            // Register the implementation to the global interface within the PCL.
            RestClient.GlobalIO = new DiskIO();

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