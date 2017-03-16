using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using System.IO;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Refractored.Controls;
using Android.Widget;
using System.Collections.Generic;
using GabberPCL;
using Android.Support.V4.Content;
using Android.Preferences;
using Android.Support.V7.Widget;
using Android.Views;
using Newtonsoft.Json;
using Android.Util;

namespace Gabber
{
	[Activity]
	public class PreparationActivity : AppCompatActivity
	{
		// The photo take by the camera activity to be stored & displayed in main.
		Java.IO.File _photo;
		List<Participant> _participants;
		List<Participant> _selectedParticipants;
		// String is type of need to override.
		Dictionary<string, ComplexNeeds> _complex_needs;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			SupportActionBar.Title = Resources.GetText(Resource.String.hint_who_gabbering_with);

			// Required to access existing gabbers for a given user
			var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

			// There are no participants selected by default
			_selectedParticipants = new List<Participant>();
			// Store these in shared prefs for simplicity of access.
			if (string.IsNullOrEmpty(prefs.GetString("participants", ""))) _participants = new List<Participant>();
			else _participants = JsonConvert.DeserializeObject<List<Participant>>(prefs.GetString("participants", ""));

			var participantsView = FindViewById<RecyclerView>(Resource.Id.participants);
			participantsView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

			var adapter = new ParticipantAdapter(_participants);
			adapter.ParticipantClicked += ParticipantSelected;
			participantsView.SetAdapter(adapter);
			participantsView.AddItemDecoration(new DividerItemDecoration(participantsView.Context, 1));
			new LinearSnapHelper().AttachToRecyclerView(participantsView);

			var name = FindViewById<TextInputEditText>(Resource.Id.participantName);
			var email = FindViewById<TextInputEditText>(Resource.Id.participantEmail);
			var photo = FindViewById<CircleImageView>(Resource.Id.prepPhoto);

			// TODO: only show this for FF deployment.
			// TODO: abstract forms to server-side.
			var age = FindViewById<TextInputEditText>(Resource.Id.age);
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


			photo.Click += delegate
			{
				// Creates a public directory to write images/audios if it does not exist as we
				// cannot read/write from internal storage as external activity is used to write the file.
				var gabberPublicDir = System.IO.Path.Combine(Environment.GetExternalStoragePublicDirectory(
					Environment.DirectoryPictures).Path, "Gabber");
				// Creates the directory if it does not exist.
				Directory.CreateDirectory(gabberPublicDir);

				_photo = new Java.IO.File(
					System.IO.Path.Combine(gabberPublicDir, System.DateTimeOffset.Now.ToUnixTimeSeconds() + ".jpg"));

				var intent = new Intent(MediaStore.ActionImageCapture);
				intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_photo));
				StartActivityForResult(intent, 0);
			};

			FindViewById<Button>(Resource.Id.addParticipant).Click += delegate
			{
				if (FormValid())
				{
					var participant = new Participant { 
						Name=name.Text, 
						Email=email.Text, 
						Gender=gender.SelectedItem.ToString(), 
						Age=age.Text,
						Needs = JsonConvert.SerializeObject(_complex_needs)
					};

					participant.Photo = (_photo != null && _photo.Length() > 0) ? _photo.AbsolutePath : "";
					_participants.Add(participant);
					_selectedParticipants.Add(participant);
					adapter.NotifyDataSetChanged();
					// TODO: set border to green
					prefs.Edit().PutString("participants", JsonConvert.SerializeObject(_participants)).Commit();
					// Reset form content once participant is successfully added
					photo.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.me));
					name.Text = "";
					email.Text = "";
				}
			};

			FindViewById<Button>(Resource.Id.selectPrompt).Click += delegate
			{
				if (_selectedParticipants.Count == 0)
				{
					Snackbar.Make(name, Resources.GetText(Resource.String.select_participant), Snackbar.LengthLong).Show();
				}
				else
				{
					prefs.Edit().PutString("participants", JsonConvert.SerializeObject(_participants)).Commit();
					// Pass the preparation form and previously form data (theme) to the record activity.
					var intent = new Intent(this, typeof(PromptSelectionActivity));
					intent.PutExtra("participants", JsonConvert.SerializeObject(_selectedParticipants));
					intent.PutExtra("theme", prefs.GetString("theme", ""));
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

		bool FormValid()
		{
			var name = FindViewById<TextInputEditText>(Resource.Id.participantName);
			var email = FindViewById<TextInputEditText>(Resource.Id.participantEmail);

			if (string.IsNullOrWhiteSpace(name.Text))
			{
				Snackbar.Make(name, Resources.GetText(Resource.String.error_friends_name), Snackbar.LengthLong).Show();
				return false;
			}
			if (!string.IsNullOrWhiteSpace(email.Text) && !Patterns.EmailAddress.Matcher(email.Text).Matches())
			{
				Snackbar.Make(email, Resources.GetText(Resource.String.error_invalid_email), Snackbar.LengthLong).Show();
				return false;
			}
			return true;
		}

		void ParticipantSelected(object sender, int position)
		{
			var participant = _participants[position];

			if (!_selectedParticipants.Contains(participant))
			{
				_selectedParticipants.Add(participant);
				ParticipantBorderColor(position, Color.Green);
			}
			else
			{
				_selectedParticipants.Remove(participant);
				ParticipantBorderColor(position, Color.Red);
			}
		}

		void ParticipantBorderColor(int itemPosition, Color color)
		{
			var participantRV = FindViewById<RecyclerView>(Resource.Id.participants);
			var viewItem = participantRV.GetLayoutManager().FindViewByPosition(itemPosition);
			var participantPhoto = viewItem.FindViewById<CircleImageView>(Resource.Id.photo);
			participantPhoto.BorderColor = color;
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			// Only perform operation if an image was taken
			if (resultCode == Result.Ok)
			{
				var photo = FindViewById<CircleImageView>(Resource.Id.prepPhoto);
				photo.Rotation = ImageRotationAngle(_photo.Path);
				// Subsample image to return smaller image to memory.
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
