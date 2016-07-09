using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
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
		Java.IO.File _photo;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			FindViewById<ImageView>(Resource.Id.photo).Click += delegate
			{
				// Creates a public directory to write images/audios if it does not exist
				var gabberPublicDir = System.IO.Path.Combine(Environment.GetExternalStoragePublicDirectory(
					Environment.DirectoryPictures).Path, "Gabber");
				// Creates the directory if it does not exist.
				Directory.CreateDirectory(gabberPublicDir);
				// Save as a timestamp in the same way as creating an audiofile.
				_photo = new Java.IO.File(System.IO.Path.Combine(gabberPublicDir, Stopwatch.GetTimestamp() + ".jpg"));
				// Cannot read/write from internal storage as external activity is used to write the file.
				var intent = new Intent(MediaStore.ActionImageCapture);
				intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_photo));
				StartActivityForResult(intent, 0);
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
					// Pass the preparation form data to the record activity.
					var intent = new Intent(this, typeof(PromptSelectionActivity));

					intent.PutExtra("photo", _photo.AbsolutePath);
					intent.PutExtra("name", name.Text);
					intent.PutExtra("email", email.Text);
					intent.PutExtra("location", "10,99"); // TODO: capture and store location data.

					// Users should return to main screen if they go back. Start over.
					Finish();
					StartActivity(intent);
				}
			};
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			var photo = FindViewById<ImageView>(Resource.Id.photo);
			var thumbnail = ThumbnailUtils.ExtractThumbnail(BitmapFactory.DecodeFile(_photo.Path), 
			                                                photo.Width, photo.Height);
			photo.SetImageBitmap(thumbnail);
		}
	}
}