using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using System.Diagnostics;
using System.IO;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Refractored.Controls;

namespace Linda
{
	[Activity(Label = "Who are you chatting with?")]
	public class PreparationActivity : AppCompatActivity
	{
		// The photo take by the camera activity to be stored & displayed in main.
		Java.IO.File _photo;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			// Need to pass an existing view to the snackbar.
			var topicSelection = FindViewById<AppCompatButton>(Resource.Id.topicSelection);
			// Make it more obvious that the silhouette is clickable.
			Snackbar.Make(topicSelection, "Why not take a photo of your friend?", Snackbar.LengthLong).Show();

			FindViewById<CircleImageView>(Resource.Id.photo).Click += delegate
			{
				// Creates a public directory to write images/audios if it does not exist
				var gabberPublicDir = System.IO.Path.Combine(Environment.GetExternalStoragePublicDirectory(
					Environment.DirectoryPictures).Path, "Gabber");
				// Creates the directory if it does not exist.
				Directory.CreateDirectory(gabberPublicDir);

				// If the user opens an activity takes a photo, then wants to re-take a photo, then write to same file.
				if (_photo == null)
				{
					// Save as a timestamp in the same way as creating an audiofile.
					_photo = new Java.IO.File(System.IO.Path.Combine(gabberPublicDir, Stopwatch.GetTimestamp() + ".jpg"));	
				}

				// Cannot read/write from internal storage as external activity is used to write the file.
				var intent = new Intent(MediaStore.ActionImageCapture);
				intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_photo));
				StartActivityForResult(intent, 0);
			};

			topicSelection.Click += delegate 
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
					// Photos are optional: this check ensures that empty files are not sent.
					// e.g. if a user takes a photo, then cancels (on the first time).
					intent.PutExtra("photo", (_photo != null && _photo.Length() > 0) ? _photo.AbsolutePath : "");
					intent.PutExtra("name", name.Text);
					intent.PutExtra("email", email.Text);
					intent.PutExtra("location", "10,99"); // TODO: capture and store location data.

					// Users should return to main screen if they go back. Start over.
					StartActivity(intent);
				}
			};
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			// Only perform operation if an image was taken
			if (resultCode == Result.Ok)
			{
				var photo = FindViewById<CircleImageView>(Resource.Id.photo);
				// Rotates the image to a horiziontal position regardless of how it was taken.
				photo.Rotation = ImageRotationAngle(_photo.Path);
				// Subsample image to return smaller image to memory.
				// TODO: fix magic sample size number.
				photo.SetImageBitmap(ThumbnailUtils.ExtractThumbnail(
					BitmapFactory.DecodeFile(_photo.Path, new BitmapFactory.Options { InSampleSize = 8 }),
					photo.Width, photo.Height));
			}
		}

		int ImageRotationAngle(string imagePath)
		{
			var exif = new ExifInterface(imagePath);
			var orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, 1);

			int rotationAngle = 0;
			if (orientation == (int)Orientation.Rotate90) rotationAngle = 90;
			if (orientation == (int)Orientation.Rotate180) rotationAngle = 180;
			if (orientation == (int)Orientation.Rotate270) rotationAngle = 270;

			return rotationAngle;
		}
	}
}