using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GabberPCL;
using Newtonsoft.Json;

namespace Gabber.Activities
{
    [Activity(Label = "Gabber to verify email")]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataScheme = "https",
        DataHost = "*gabber.audio",
        DataPathPrefix = "/verify/"
    )]
    public class VerifyEmail : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_verification_res);

            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            // The user has already been logged in -- not sure how they got here.
            if (!string.IsNullOrWhiteSpace(prefs.GetString("username", "")))
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }

            // Shown if an error occurs, such as account already been verified.
            var loginButton = FindViewById<AppCompatButton>(Resource.Id.registerVerifyLogin);

            // The URI click in the email
            var dataURI = base.Intent.Data;

            if (!string.IsNullOrEmpty(dataURI.ToString()))
            {
                FindViewById<ProgressBar>(Resource.Id.registerVerifyProgressBar).Visibility = ViewStates.Visible;
                var response = await new RestClient().RegisterVerify(dataURI.LastPathSegment);
                FindViewById<ProgressBar>(Resource.Id.registerVerifyProgressBar).Visibility = ViewStates.Gone;

                if (response.Meta.Success)
                {
                    prefs.Edit().PutString("username", response.Data.User.Email).Commit();
                    prefs.Edit().PutString("tokens", JsonConvert.SerializeObject(response.Data.Tokens)).Commit();
                    Queries.SetActiveUser(response.Data);

                    var intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(intent);
                    Finish();
                }
                else
                {
                    loginButton.Visibility = ViewStates.Visible;
                    FindViewById<TextView>(Resource.Id.registerVerifyBody).Text = "There was an error.";
                    ToastErrors(response.Meta.Messages);
                }
            }

            loginButton.Click += delegate {
                var intent = new Intent(this, typeof(LoginActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
                Finish();
            };
        }

        void ToastErrors(List<string> errors)
        {
            var view = FindViewById<TextView>(Resource.Id.registerVerifyBody);
            errors.ForEach((err) => Snackbar.Make(view, err, Snackbar.LengthLong).Show());
        }
    }
}
