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

namespace Gabber.Activities
{
	[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ConversationConsent : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.consent_conversation);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = StringResources.consent_gabber_toolbar_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var selectedProject = Queries.ProjectById(_prefs.GetInt("SelectedProjectID", 0));

            var controlTitle = FindViewById<TextView>(Resource.Id.GabberConsentControlTitle);
            controlTitle.Text = StringResources.consent_gabber_title_control;
            var controlDesc = FindViewById<TextView>(Resource.Id.GabberConsentControlDesc);
            controlDesc.TextFormatted = Html.FromHtml(StringResources.consent_gabber_body_control);

            var decisionTitle = FindViewById<TextView>(Resource.Id.GabberConsentDecisionTitle);
            decisionTitle.Text = StringResources.consent_gabber_title_decision;
            var decisionDesc = FindViewById<TextView>(Resource.Id.GabberConsentDecisionDesc);
            decisionDesc.Text = StringResources.consent_gabber_body_decision;

            var consentTypePublic = FindViewById<RadioButton>(Resource.Id.GabberConsentTypePublic);
            consentTypePublic.Text = StringResources.consent_gabber_consent_type_public_brief;

            var consentTypePublicFull = FindViewById<TextView>(Resource.Id.GabberConsentTypePublicFull);
            consentTypePublicFull.Text = StringResources.consent_gabber_consent_type_public_full;

            var consentTypeMembers = FindViewById<RadioButton>(Resource.Id.GabberConsentTypeMembers);
            consentTypeMembers.Text = StringResources.consent_gabber_consent_type_members_brief;

            var consentTypeMembersFull = FindViewById<TextView>(Resource.Id.GabberConsentTypeMembersFull);
            consentTypeMembersFull.TextFormatted = Html.FromHtml(
                string.Format(StringResources.consent_gabber_consent_type_members_full, 
                              selectedProject.Members.Count,
                              selectedProject.Members.FindAll((obj) => obj.Role == "researcher").Count));

            var consentTypePrivate = FindViewById<RadioButton>(Resource.Id.GabberConsentTypePrivate);
            consentTypePrivate.Text = StringResources.consent_gabber_consent_type_private_brief;

            var consentTypePrivateFull = FindViewById<TextView>(Resource.Id.GabberConsentTypePrivateFull);
            var participants = BuildParticipants(Queries.SelectedParticipants().ToList());
            consentTypePrivateFull.TextFormatted = Html.FromHtml(string.Format(StringResources.consent_gabber_consent_type_private_full, participants));

            var isConsented = FindViewById<RadioGroup>(Resource.Id.GabberConsentProvided);

            var submit = FindViewById<AppCompatButton>(Resource.Id.GabberConsentSubmit);
            submit.Text = StringResources.consent_gabber_submit;
            submit.Enabled = false;

            submit.Click += (s, e) => 
            { 
                // TODO: If (consentTypeMembers.Checked || consentTypePrivate.Checked) 
                // then show dialog to illustrate the affect of this choice.
                var consent = "";
                if (consentTypePublic.Checked)  consent = "public";
                if (consentTypeMembers.Checked) consent = "members";
                if (consentTypePrivate.Checked) consent = "private";
                // This is used then deleted when saving the recording session
                _prefs.Edit().PutString("SESSION_CONSENT", consent).Commit();
                StartActivity(new Intent(this, typeof(RecordStoryActivity))); 
            };

            isConsented.CheckedChange += (s, e) => { submit.Enabled = true; };
		}

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