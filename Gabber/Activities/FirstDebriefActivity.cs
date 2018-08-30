using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Content.PM;
using Android.Support.Text.Emoji.Widget;
using Android.Support.Text.Emoji;
using GabberPCL.Resources;
using Android.Support.V7.Widget;
using Android.Content;
using Android.Preferences;

namespace Gabber.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class FirstDebriefActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.FirstDebriefActivity);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = StringResources.debriefing_activity_title;

            var emojiTextView = (EmojiAppCompatTextView)FindViewById(Resource.Id.emoji_text_view);
            emojiTextView.Text = EmojiCompat.Get().Process("\uD83C\uDF89");

            FindViewById<TextView>(Resource.Id.CongratsTitle).Text = StringResources.debriefing_congrats_title;
            FindViewById<TextView>(Resource.Id.CongratsBody).Text = string.Format(
                StringResources.debriefing_congrats_body, GabberPCL.Config.PRINT_URL);

            FindViewById<TextView>(Resource.Id.ConsentTitle).Text = StringResources.debriefing_consent_title;
            FindViewById<TextView>(Resource.Id.ConsentBody1).Text = StringResources.debriefing_consent_body1;
            FindViewById<TextView>(Resource.Id.ConsentBody2).Text = StringResources.debriefing_consent_body2;
            FindViewById<TextView>(Resource.Id.ConsentBody3).Text = StringResources.debriefing_consent_body3;

            AppCompatButton finishButton = FindViewById<AppCompatButton>(Resource.Id.finishButton);
            finishButton.Text = StringResources.debriefing_finish_button;
            finishButton.Click += FinishButton_Click;
        }

        private void FinishButton_Click(object sender, System.EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("HAS_DISMISSED_DEBRIEF", true);
            editor.Apply();
            Finish();
        }

    }
}
