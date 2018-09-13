using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Resources;

namespace Gabber.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, ParentActivity = typeof(ChooseParticipantsActivity))]
    public class ResearchConsent : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Localise.SetLayoutDirectionByPreference(this);
            SetContentView(Resource.Layout.consent_research);

            SupportActionBar.Title = StringResources.consent_research_toolbar_title;

            var _prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var SelectedProjectID = _prefs.GetInt("SelectedProjectID", 0);
            var selectedProject = Queries.ProjectById(SelectedProjectID);
            // If there is no organisation then the project was created by an individual.
            var IsOrg = selectedProject.Organisation.Name.ToLower() == "individual";
            var org = IsOrg ? selectedProject.Creator.Name : selectedProject.Organisation.Name;

            var researchConsentDesc = FindViewById<TextView>(Resource.Id.researchConsentDesc);
            var title = Localise.ContentByLanguage(selectedProject).Title;
            researchConsentDesc.Text = string.Format(StringResources.consent_research_body, org, title);

            var researchConsentForm = FindViewById<TextView>(Resource.Id.researchConsentForm);
            researchConsentForm.Text = StringResources.consent_research_form;

            AppCompatButton moreInfoBtn = FindViewById<AppCompatButton>(Resource.Id.consentInfoButton);
            moreInfoBtn.Text = StringResources.consent_research_details_button;
            moreInfoBtn.Click += ViewConsentDetails;

            var submit = FindViewById<AppCompatButton>(Resource.Id.researchConsentSubmit);
            submit.Text = StringResources.consent_research_submit;
            submit.Enabled = false;
            submit.Click += (s, e) =>
            {
                StartActivity(new Intent(this, typeof(ConversationConsent)));
            };

            var isConsented = FindViewById<CheckBox>(Resource.Id.researchConsentProvided);
            isConsented.Click += (s, e) => { submit.Enabled = isConsented.Checked; };

            var form = FindViewById<LinearLayout>(Resource.Id.researchConsentFormLayout);
            form.Click += (s, e) =>
            {
                isConsented.Toggle();
                submit.Enabled = isConsented.Checked;
            };
        }

        private void ViewConsentDetails(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse(Config.ABOUT_DATA_PAGE));
            StartActivity(intent);
        }

    }
}