using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Media;
using System.IO;

namespace Linda
{
	[Activity(Label = "Record their story")]
	public class RecordStoryActivity : AppCompatActivity
	{
		MediaRecorder _recorder;
		// Gosh: https://code.google.com/p/android/issues/detail?id=800
		bool _isrecording;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.record);

			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			// Enable a user to modify the interviewees details if they wish
			// TODO: this should only be enabled once a recording has been made.
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetHomeButtonEnabled(true);

			FindViewById<AppCompatButton>(Resource.Id.start).Click += delegate
			{
				_recorder = new MediaRecorder();
				_isrecording = true;

				// Set how we want the audio formatting to be.
				_recorder.SetAudioSource(AudioSource.Mic);
				_recorder.SetOutputFormat(OutputFormat.ThreeGpp);
				_recorder.SetAudioEncoder(AudioEncoder.AmrNb);

				// TODO: save path to database for the last use
				var path = Path.Combine(
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
					System.Diagnostics.Stopwatch.GetTimestamp() + ".3gpp");

				_recorder.SetOutputFile(path);
				_recorder.Prepare();
				_recorder.Start();
			};

			FindViewById<AppCompatButton>(Resource.Id.stop).Click += delegate
			{
				if (_isrecording)
				{
					_recorder.Stop();
					_recorder.Reset();	
				}
			};

			FindViewById<AppCompatButton>(Resource.Id.submit).Click += delegate
			{
				// TODO: an audio must have been created! Enable once recording made?
				// We do not want the user to return to this page
				// Once a story is created, then it is created.
				Finish();
				StartActivity(typeof(MainActivity));
			};
		}
	}
}