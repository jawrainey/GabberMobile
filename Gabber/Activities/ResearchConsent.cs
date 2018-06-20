using Android.App;
using Android.OS;
using Android.Support.V7.App;
using GabberPCL.Resources;
using Android.Content.PM;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Content;
using Android.Views;
using GabberPCL;
using Android.Text;

namespace Gabber.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ResearchConsent : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.consent_research);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = StringResources.consent_research_toolbar_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var researchConsentTitle = FindViewById<TextView>(Resource.Id.researchConsentTitle);
            researchConsentTitle.Text = StringResources.consent_research_title;

            var _prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var SelectedProjectID = _prefs.GetInt("SelectedProjectID", 0);
            var selectedProject = Queries.ProjectById(SelectedProjectID);
            // If there is no organisation then the project was created by an individual.
            var IsOrg = selectedProject.Organisation.Name.ToLower() == "individual";
            var org = IsOrg ? selectedProject.Creator.Name : selectedProject.Organisation.Name;

            var researchConsentDesc = FindViewById<TextView>(Resource.Id.researchConsentDesc);
            researchConsentDesc.TextFormatted = Html.FromHtml(string.Format(StringResources.consent_research_body, selectedProject.Title, org));

            var researchConsentForm = FindViewById<TextView>(Resource.Id.researchConsentForm);
            researchConsentForm.Text = StringResources.consent_research_form;

            var submit = FindViewById<AppCompatButton>(Resource.Id.researchConsentSubmit);
            submit.Text = StringResources.consent_research_submit;
            submit.Enabled = false;
            submit.Click += (s, e) => { StartActivity(new Intent(this, typeof(ConversationConsent))); };

            var isConsented = FindViewById<CheckBox>(Resource.Id.researchConsentProvided);
            isConsented.Click += (s, e) => { submit.Enabled = isConsented.Checked; };

            var form = FindViewById<LinearLayout>(Resource.Id.researchConsentFormLayout);
            form.Click += (s, e) =>
            {
                isConsented.Toggle();
                submit.Enabled = isConsented.Checked;
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            OnBackPressed();
            return true;
        }
    }
}