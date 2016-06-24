using Android.App;
using Android.Media;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using System;
using System.Collections.Generic;
using System.IO;

namespace Linda
{
	[Activity(Label = "Your stories", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		public static string STATE = "";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			// TODO: use shared preferences to keep check if a user is logged in.
			if (string.IsNullOrWhiteSpace(STATE))
			{
				StartActivity(typeof(LoginActivity));
				Finish();
			}

			// One MediaPlayer to rule the view.
			var mplayer = new MediaPlayer();

			// TODO: acquire these from a local database
			// The stories (experiences) the user has gathered from other people.
			var _stories = new List<Tuple<string, string>>();

			// Datafiles are stored in personal folder for simplicity.
			var personal = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

			// TODO: populate this from database contents
			foreach (var file in Directory.GetFiles(personal, "*.3gpp"))
				_stories.Add(new Tuple<string, string>("hello", file));

			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.main);

			// Toolbar will now take on default actionbar characteristics
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			var rv = FindViewById<RecyclerView>(Resource.Id.stories);
			rv.SetLayoutManager(new LinearLayoutManager(this));
			rv.SetAdapter(new RecyclerAdapter(_stories, mplayer));

			FindViewById<FloatingActionButton>(Resource.Id.fab).Click += delegate
			{
				StartActivity(typeof(PreparationActivity));
			};
		}
	}
}