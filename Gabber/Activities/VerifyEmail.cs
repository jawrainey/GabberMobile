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
using GabberPCL.Resources;
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

            var _title = FindViewById<TextView>(Resource.Id.registerVerifyTitle);
            _title.Text = StringResources.register_verifying_ui_page_title;
            var _content = FindViewById<TextView>(Resource.Id.registerVerifyContent);
            _content.Text = StringResources.register_verifying_ui_page_content;

            // Shown if an error occurs, such as account already been verified.
            var loginButton = FindViewById<AppCompatButton>(Resource.Id.registerVerifyLogin);
            loginButton.Text = StringResources.register_verifying_ui_button_login;

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
                    FindViewById<TextView>(Resource.Id.registerVerifyContent).Text = StringResources.register_verifying_ui_page_content_error;
                    response.Meta.Messages.ForEach(MakeError);
                }
            }

            loginButton.Click += delegate {
                var intent = new Intent(this, typeof(LoginActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
                Finish();
            };
        }

        void MakeError(string errorMessage)
        {
            var view = FindViewById<TextView>(Resource.Id.registerVerifyContent);
            var message = StringResources.ResourceManager.GetString($"register.verifying.api.error.{errorMessage}");
            Snackbar.Make(view, message, Snackbar.LengthLong).Show();
        }
    }
}
