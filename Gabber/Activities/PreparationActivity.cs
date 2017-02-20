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
using Android.Support.V7.Widget;
using Android.Views;

namespace Gabber
{
	[Activity(Label = "Who are you gabbering with?")]
	public class PreparationActivity : AppCompatActivity
	{
		// The photo take by the camera activity to be stored & displayed in main.
		Java.IO.File _photo;
		// Provide access for the spinner methods.
		List<Story> _stories;
		// String is type of need to override.
		Dictionary<string, ComplexNeeds> _complex_needs;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			// Need to pass an existing view to the snackbar.
			var topicSelection = FindViewById<AppCompatButton>(Resource.Id.submit);
			// Make it more obvious that the silhouette is clickable.
			Snackbar.Make(topicSelection, "Who is participating in the interview?", Snackbar.LengthLong).Show();

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
			friends.Insert(0, "Previous participants...");
			var spinnerAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, friends);
			spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = spinnerAdapter;

			var genders = new List<string> { "Gender", "Female", "Male", "Other" };
			var genderAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, genders);
			genderAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			var gender = FindViewById<Spinner>(Resource.Id.gender);
			gender.Adapter = genderAdapter;

			// Stored the selected needs to pass between activities
			_complex_needs = new Dictionary<string, ComplexNeeds>();

			var substance = FindViewById<CheckBox>(Resource.Id.substance);
			substance.Click += (o, e) =>
			{
				if (substance.Checked) ComplexNeedsDialog("substance");
				else _complex_needs.Remove("substance");
			};

			var housing = FindViewById<CheckBox>(Resource.Id.housing);
			housing.Click += (o, e) =>
			{
				if (housing.Checked) ComplexNeedsDialog("housing");
				else _complex_needs.Remove("housing");
			};

			var mental = FindViewById<CheckBox>(Resource.Id.mental);
			mental.Click += (o, e) =>
			{
				if (mental.Checked) ComplexNeedsDialog("mental");
				else _complex_needs.Remove("mental");
			};

			var police = FindViewById<CheckBox>(Resource.Id.police);
			police.Click += (o, e) =>
			{
				if (police.Checked) ComplexNeedsDialog("police");
				else _complex_needs.Remove("police");
			};

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
				var age = FindViewById<TextInputEditText>(Resource.Id.age);

				if (string.IsNullOrWhiteSpace(name.Text))
				{
					Snackbar.Make(email, "A name is required.", Snackbar.LengthLong).Show();
				}
				else if (email.Text.Length > 4 && !Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
					Snackbar.Make(email, "The email address entered is invalid.", Snackbar.LengthLong).Show();
				}
				else if (string.IsNullOrEmpty(age.Text) || int.Parse(age.Text) <= 0 || int.Parse(age.Text) >= 100)
				{
					Snackbar.Make(email, "An age is required", Snackbar.LengthLong).Show();
				}
				else if (gender.SelectedItemPosition == 0)
				{
					Snackbar.Make(email, "A gender is required.", Snackbar.LengthLong).Show();
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
					intent.PutExtra("age", age.Text);
					intent.PutExtra("gender", gender.SelectedItem.ToString());
					intent.PutExtra("needs", Newtonsoft.Json.JsonConvert.SerializeObject(_complex_needs));
					// Pass the previous form data (selected theme)
					intent.PutExtra("theme", PreferenceManager.GetDefaultSharedPreferences(
						ApplicationContext).GetString("theme", ""));
					// Users should return to main screen if they go back. Start over.
					StartActivity(intent);
				}
			};
		}

		void ComplexNeedsDialog(string type)
		{
			var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
			var dialogView = LayoutInflater.Inflate(Resource.Layout.complexneeds, null);
			builder.SetView(dialogView);
			builder.SetTitle("Details of your experience");

			var lt = dialogView.FindViewById<TextView>(Resource.Id.longagotext);
			var lg = dialogView.FindViewById<GridLayout>(Resource.Id.longagogrid);
			var previous = dialogView.FindViewById<RadioButton>(Resource.Id.previous);

			// Only show the year/month when previous is selected 
			previous.CheckedChange += delegate
			{
				lt.Visibility = lt.Visibility == ViewStates.Gone ? ViewStates.Visible : ViewStates.Gone;
				lg.Visibility = lg.Visibility == ViewStates.Gone ? ViewStates.Visible : ViewStates.Gone;
			};


			var rbg = dialogView.FindViewById<RadioGroup>(Resource.Id.rbgroup);
			var year = dialogView.FindViewById<TextInputEditText>(Resource.Id.yearinput);
			var month = dialogView.FindViewById<TextInputEditText>(Resource.Id.monthinput);

			// Previously stored information for this particular complex need
			if (_complex_needs.ContainsKey(type))
			{
				year.Text = _complex_needs[type].year;
				month.Text = _complex_needs[type].month;
				var tl = _complex_needs[type].timeline;

				if (tl.Contains("current")) rbg.Check(Resource.Id.current);
				else rbg.Check(Resource.Id.previous);
			}

			builder.SetPositiveButton(Android.Resource.String.Ok, (sender, e) => 
			{
				var cn = new ComplexNeeds();
				cn.type = type;
				cn.timeline = dialogView.FindViewById<RadioButton>(rbg.CheckedRadioButtonId).Text.ToLower();
				// If "current" is chosen, then no month/year is shown.
				cn.month = string.IsNullOrEmpty(month.Text) ? "0" : month.Text;
				cn.year = string.IsNullOrEmpty(year.Text) ? "0" : year.Text;
				// Override existing need if it is re-selected
				if (_complex_needs.ContainsKey(type)) _complex_needs["type"] = cn;
				else _complex_needs.Add(type, cn);
			});

			builder.Create().Show();
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
			FindViewById<TextInputEditText>(Resource.Id.age).Text = match.IntervieweeAge;
			var genders = new List<string> { "Gender", "Female", "Male", "Other" };
			FindViewById<Spinner>(Resource.Id.gender).SetSelection(genders.FindIndex(x => x == match.IntervieweeGender));

			var photo = FindViewById<CircleImageView>(Resource.Id.photo);
			Snackbar.Make(photo, "Complex needs must be re-entered for each interview", Snackbar.LengthLong).Show();

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