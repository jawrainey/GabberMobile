using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Content;
using Android.Views;
using GabberPCL.Resources;
using Android.Text;
using Android.Preferences;
using GabberPCL.Models;
using GabberPCL;
using System.Linq;
using Gabber.Adapters;
using System.Collections.Generic;
using Java.Lang;

namespace Gabber.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ConsentSummary : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.consent_summary);

            SupportActionBar.Title = StringResources.consent_summary_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var selectedProject = Queries.ProjectById(_prefs.GetInt("SelectedProjectID", 0));

            var desc = FindViewById<TextView>(Resource.Id.ConsentSummaryDesc);
            desc.Text = StringResources.consent_summary_subtitle;

            var projectContent = FindViewById<TextView>(Resource.Id.ConsentSummaryContentProject);
            projectContent.TextFormatted = BuildHTML(StringResources.consent_summary_screen_content_project_title, selectedProject.Title);

            var partsContent = FindViewById<TextView>(Resource.Id.ConsentSummaryContentParticipants);
            var participantsDesc = string.Format(StringResources.consent_summary_screen_content_participants_desc, BuildParticipants(Queries.SelectedParticipants()));
            partsContent.TextFormatted = BuildHTML(StringResources.consent_summary_screen_content_participants_title, participantsDesc);

            var researchContent = FindViewById<TextView>(Resource.Id.ConsentSummaryContentResearch);
            researchContent.TextFormatted = BuildHTML(StringResources.consent_summary_screen_content_research_title, StringResources.consent_summary_screen_content_research_desc);

            var convoContent = FindViewById<TextView>(Resource.Id.ConsentSummaryContentConversation);
            convoContent.TextFormatted = BuildHTML(
                StringResources.consent_summary_screen_content_conversation_title,
                string.Format(StringResources.consent_summary_screen_content_conversation_desc, _prefs.GetString("SESSION_CONSENT", "public"))
            );

            var embargoContent = FindViewById<TextView>(Resource.Id.ConsentSummaryEmbargo);
            embargoContent.TextFormatted = BuildHTML(StringResources.consent_summary_screen_content_embargo_title, StringResources.consent_summary_screen_content_embargo_desc);

            var submit = FindViewById<AppCompatButton>(Resource.Id.ConsentSummarySubmit);
            submit.Text = StringResources.consent_summary_screen_action;
            submit.Click += (s, e) => StartActivity(new Intent(this, typeof(RecordStoryActivity)));
        }

        ICharSequence BuildHTML(string title, string content) => Html.FromHtml($"&bullet; <b>{title}:</b> {content}");

        string BuildParticipants(List<User> participants)
        {
            if (participants.Count == 1) return participants[0].Name.Split(' ')[0];
            var PartNames = new List<string>();
            foreach (var p in participants) PartNames.Add(p.Name.Split(' ')[0].Trim());
            return string.Join(", ", PartNames);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            OnBackPressed();
            return true;
        }
    }
}