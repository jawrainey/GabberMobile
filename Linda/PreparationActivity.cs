using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using System.Diagnostics;
using System.IO;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Linda
{
	[Activity(Label = "Story preparation")]
	public class PreparationActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			FindViewById<ImageView>(Resource.Id.photo).Click += delegate
			{
				// TODO: could not store intent data to private directory -- infuriating.
				// Perhaps this is a permissions issue? Another (external) activity
				// may require permissions to 
				StartActivityForResult(new Intent(MediaStore.ActionImageCapture), 0);
			};

			FindViewById<AppCompatButton>(Resource.Id.record_story).Click += delegate 
			{
				// TODO: VALIDATE INPUT (is everything completed?)
				// TODO: capture all data to be either (1) saved locally or (2) sent next
				// TODO: this is the place we capture location data

				// TODO: if we want to enable the user to come back to this activity
				// then we must pull in the last inserted row into the database,
				// to which they can modify information.
				// This is fine, and assumes that they will not go back again to main
				// Can we only populate it with data fro

				// TODO: WHAT IF THEY GO BACK FROM HERE TO THE MAIN SCREEN?
				// DO WE ASSUME THAT THEY WANTED TO QUIT, AND NOT CACHE ANY DATA?
				// WE CAN ASSUME THAT FOR NOW, FOR SIMPLICITY, NO?
				StartActivity(typeof(RecordStoryActivity));
			};
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			// Conver all the data to bitmap, which is unfortunately a thumbnail.
			var author = (Bitmap) data.Extras.Get("data");
			// Store locally as we do not want
			var personal = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			// Compressed and saves all at once!
			author.Compress(Bitmap.CompressFormat.Jpeg, 100, new FileStream(
				System.IO.Path.Combine(personal, Stopwatch.GetTimestamp() + ".jpg"), FileMode.CreateNew));
			// Update the default image with the one the user just took.
			FindViewById<ImageView>(Resource.Id.photo).SetImageBitmap(author);
		}
	}
}