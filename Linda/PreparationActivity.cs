using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Support.Design.Widget;
using System.Diagnostics;
using System.IO;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Linda
{
	[Activity(Label = "S2: who are they?")]
	public class PreparationActivity : AppCompatActivity
	{
		// The photo take by the camera activity to be stored & displayed in main.
		string _photo;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			FindViewById<ImageView>(Resource.Id.photo).Click += delegate
			{
				// TODO: could not store intent data to private directory -- infuriating.
				// NOTE: it is not possible to save ImageCapture to private directory.
				StartActivityForResult(new Intent(MediaStore.ActionImageCapture), 0);
			};

			FindViewById<AppCompatButton>(Resource.Id.record_story).Click += delegate 
			{
				var name = FindViewById<TextInputEditText>(Resource.Id.name);
				var email = FindViewById<TextInputEditText>(Resource.Id.email);

				// We only care about their email to "pass-it-on".
				if (string.IsNullOrWhiteSpace(email.Text))
				{
					Snackbar.Make(email, "Your friends email is required.", Snackbar.LengthLong).Show();
				}
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
					Snackbar.Make(email, "That email address is invalid.", Snackbar.LengthLong).Show();
				}
				else
				{
					// TODO: there's probably a tidier way to get checkbox information
					string consent = "";
					if (FindViewById<CheckBox>(Resource.Id.public_story).Checked) consent += "P";
					if (FindViewById<CheckBox>(Resource.Id.public_photo).Checked) consent += "F";

					// Pass the preparation form data to the record activity.
					var intent = new Intent(this, typeof(PromptSelectionActivity));

					intent.PutExtra("photo", _photo);
					intent.PutExtra("name", name.Text);
					intent.PutExtra("email", email.Text);
					intent.PutExtra("consent", consent);
					intent.PutExtra("location", "10,99"); // TODO: capture and store location data.

					// Users should return to main screen if they go back. Start over.
					Finish();
					StartActivity(intent);
				}
			};
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			// TODO: image is currently stored as a thumbnail.
			base.OnActivityResult(requestCode, resultCode, data);
			// Conver all the data to bitmap, which is unfortunately a thumbnail.
			var author = (Bitmap) data.Extras.Get("data");
			// Store locally as we do not want
			var personal = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			_photo = System.IO.Path.Combine(personal, Stopwatch.GetTimestamp() + ".jpg");
			// Compressed and saves all at once!
			author.Compress(Bitmap.CompressFormat.Jpeg, 100, new FileStream(_photo, FileMode.CreateNew));
			// Update the default image with the one the user just took.
			FindViewById<ImageView>(Resource.Id.photo).SetImageBitmap(author);
		}
	}
}