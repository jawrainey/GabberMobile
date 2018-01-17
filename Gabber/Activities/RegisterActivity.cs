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

namespace Gabber
{
	[Activity]
	public class RegisterActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.register);


			FindViewById<AppCompatButton>(Resource.Id.submit).Click += async delegate
			{
				var fname = FindViewById<AppCompatEditText>(Resource.Id.name);
				var email = FindViewById<AppCompatEditText>(Resource.Id.email);
				var passw = FindViewById<AppCompatEditText>(Resource.Id.password);

				// TODO: snackbars are used for simplicity. Ideally, specific error messages
				// would be output for each unique error instead of a generic (informative) message.
				if (string.IsNullOrWhiteSpace(fname.Text) ||
					string.IsNullOrWhiteSpace(email.Text) ||
					string.IsNullOrWhiteSpace(passw.Text))
				{
					Snackbar.Make(email, Resources.GetText(Resource.String.error_all_details), Snackbar.LengthLong).Show();
				}
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
					Snackbar.Make(email, Resources.GetText(Resource.String.error_invalid_email), Snackbar.LengthLong).Show();
				}
				else
				{
					FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Visible;
					FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = false;

					if (await new RestClient().Register(fname.Text, email.Text, passw.Text))
					{
                        var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

                        prefs.Edit().PutString("name", fname.Text).Commit();
                        prefs.Edit().PutString("username", email.Text).Commit();
                        // This allows the current user (who registered) to be displayed in the participants view
                        var user = new Participant
                        {
                            Name = "(You)",
                            Email = email.Text,
                            Selected = true
                        };
                        var _participants = new System.Collections.Generic.List<Participant> { user };
                        var _parts_as_json = Newtonsoft.Json.JsonConvert.SerializeObject(_participants);
                        prefs.Edit().PutString("participants", _parts_as_json).Commit();

						var intent = new Intent(this, typeof(MainActivity));
						intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
						StartActivity(intent);
						Finish();
					}
					else
					{
						RunOnUiThread(() =>
						{
							FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = true;
							FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Gone;
						});

						Snackbar.Make(email, Resources.GetText(Resource.String.oh_my), 0).Show();
					}
				}
			};
		}
	}
}