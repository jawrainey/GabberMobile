using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using System.Diagnostics;
using System.IO;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Refractored.Controls;
using Android.Widget;
using System.Collections.Generic;
using GabberPCL;
using System.Linq;
using Android.Support.V4.Content;
using Android.Preferences;

namespace Gabber
{
	[Activity]
	public class PreparationActivity : AppCompatActivity
	{
		// The photo take by the camera activity to be stored & displayed in main.
		Java.IO.File _photo;
		// Provide access for the spinner methods.
		List<Story> _stories;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			SupportActionBar.Title = Resources.GetText(Resource.String.hint_who_gabbering_with);

			// Need to pass an existing view to the snackbar.
			var topicSelection = FindViewById<FloatingActionButton>(Resource.Id.topicSelectionFAB);
			// Make it more obvious that the silhouette is clickable.
			Snackbar.Make(topicSelection, Resources.GetText(Resource.String.hint_who_interview), Snackbar.LengthLong).Show();

			// Required to access existing gabbers for a given user
			var model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
			var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
			_stories = model.GetStories(prefs.GetString("username", ""));

			var spinner = FindViewById<Spinner>(Resource.Id.previousFriends);
			spinner.ItemSelected += PreviousIntervieweeSelected;
			if (_stories.Count <= 0)
			{
				spinner.Enabled = false;
				spinner.Clickable = false;
			}

			var friends = PreviousFriends();
			friends.Insert(0, Resources.GetText(Resource.String.previous_participants));
			var spinnerAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, friends);
			spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = spinnerAdapter;

			FindViewById<CircleImageView>(Resource.Id.photo).Click += delegate
			{
				// Creates a public directory to write images/audios if it does not exist
				var gabberPublicDir = System.IO.Path.Combine(Environment.GetExternalStoragePublicDirectory(
					Environment.DirectoryPictures).Path, "Gabber");
				// Creates the directory if it does not exist.
				Directory.CreateDirectory(gabberPublicDir);

				// If the user opens an activity takes a photo, then wants to re-take a photo, then write to same file.
				// Save as a timestamp in the same way as creating an audiofile.
				_photo = new Java.IO.File(System.IO.Path.Combine(gabberPublicDir, Stopwatch.GetTimestamp() + ".jpg"));

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
					Snackbar.Make(email, Resources.GetText(Resource.String.error_friends_email), Snackbar.LengthLong).Show();
				}
				else if (string.IsNullOrWhiteSpace(name.Text))
				{
					Snackbar.Make(email, Resources.GetText(Resource.String.error_friends_name), Snackbar.LengthLong).Show();
				}
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
					Snackbar.Make(email, Resources.GetText(Resource.String.error_invalid_email), Snackbar.LengthLong).Show();
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
					// Pass the previous form data (selected theme)
					intent.PutExtra("theme", PreferenceManager.GetDefaultSharedPreferences(
						ApplicationContext).GetString("theme", ""));
					// Users should return to main screen if they go back. Start over.
					StartActivity(intent);
				}
			};
		}

		List<string> PreviousFriends()
		{
			// Using hash to prevent duplicate names if a person was interviewed multiple times.
			var hash = new HashSet<string>();
			foreach (var str in _stories.ConvertAll((Story s) => s.IntervieweeName)) hash.Add(str);
			return hash.ToList();
		}

		void PreviousIntervieweeSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			// The first element are the instructions
			if (e.Position <= 0) return;
			// The selected name and related email to populate the form with
			var intervieweeName = ((Spinner)sender).GetItemAtPosition(e.Position).ToString();
			var match = _stories.Find((s) => s.IntervieweeName == intervieweeName);
			var intervieweeEmail = match.IntervieweeEmail;
			FindViewById<TextInputEditText>(Resource.Id.name).Text = intervieweeName;
			FindViewById<TextInputEditText>(Resource.Id.email).Text = intervieweeEmail;
			var photo = FindViewById<CircleImageView>(Resource.Id.photo);
			// Use previously taken photo
			if (!string.IsNullOrEmpty(match.PhotoPath))
			{
				var previousPhoto = new Java.IO.File(match.PhotoPath);
				_photo = previousPhoto;
				photo.SetImageURI(Android.Net.Uri.FromFile(_photo));
				photo.Rotation = ImageRotationAngle(match.PhotoPath);
			}
			else
			{
				photo.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.me));
			}

			// Make obvious that reselecting will change content of form.
			((Spinner)sender).SetSelection(0);
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
			if (orientation == (int)Android.Media.Orientation.Rotate90) rotationAngle = 90;
			if (orientation == (int)Android.Media.Orientation.Rotate180) rotationAngle = 180;
			if (orientation == (int)Android.Media.Orientation.Rotate270) rotationAngle = 270;

			return rotationAngle;
		}
	}
}