using Android.App;
using Android.Content;
using Android.OS;
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

            FindViewById<TextInputEditText>(Resource.Id.password).EditorAction += (_, e) => {
                e.Handled = false;
                if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
                {
                    FindViewById<AppCompatButton>(Resource.Id.submit).PerformClick();
                    e.Handled = true;
                }
            };

			FindViewById<AppCompatButton>(Resource.Id.submit).Click += async delegate
			{
                var imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(FindViewById<TextInputEditText>(Resource.Id.password).WindowToken, 0);

				var fname = FindViewById<AppCompatEditText>(Resource.Id.name);
				var email = FindViewById<AppCompatEditText>(Resource.Id.email);
				var passw = FindViewById<AppCompatEditText>(Resource.Id.password);

                if (string.IsNullOrWhiteSpace(fname.Text))
				{
                    fname.RequestFocus();
					Snackbar.Make(email, "Your Full Name is empty", Snackbar.LengthLong).Show();
				}
                else if (string.IsNullOrWhiteSpace(email.Text))
                {
                    email.RequestFocus();
                    Snackbar.Make(email, "An Email Address is empty", Snackbar.LengthLong).Show();
                }
                else if (string.IsNullOrWhiteSpace(passw.Text))
                {
                    passw.RequestFocus();
                    Snackbar.Make(email, "A password for your account is required", Snackbar.LengthLong).Show();
                }
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
                    email.RequestFocus();
                    Snackbar.Make(email, "The email provided is invalid", Snackbar.LengthLong).Show();
				}
				else
				{
					FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Visible;
					FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = false;

                    var api = new RestClient();
                    var isRegisterSuccess = await api.Register(
                        fname.Text, 
                        email.Text.ToLower(), 
                        passw.Text, 
                        (errorMessage) => Snackbar.Make(email, errorMessage, 0).Show()
                    );

                    if (isRegisterSuccess)
					{
                        var intent = new Intent(this, typeof(Activities.RegisterVerification));
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
                            fname.RequestFocus();
						});
					}
				}
			};
		}
    }
}