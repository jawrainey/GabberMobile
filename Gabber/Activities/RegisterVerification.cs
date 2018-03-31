using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace Gabber.Activities
{
    [Activity]
    public class RegisterVerification : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_verification);

            FindViewById<AppCompatButton>(Resource.Id.openEmail).Click += delegate
            {
                Intent intent = new Intent(Intent.ActionMain);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.AddCategory(Intent.CategoryAppEmail);
                StartActivity(intent);
            };
        }
	}
}