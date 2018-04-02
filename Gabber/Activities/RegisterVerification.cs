using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using GabberPCL.Resources;

namespace Gabber.Activities
{
    [Activity]
    public class RegisterVerification : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_verification);
            FindViewById<TextView>(Resource.Id.verifyTitle).Text = StringResources.register_verify_ui_page_title;
            FindViewById<TextView>(Resource.Id.verifyContent).Text = StringResources.register_verify_ui_page_content;
            FindViewById<AppCompatButton>(Resource.Id.openEmail).Text = StringResources.register_verify_ui_button_openemail;

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