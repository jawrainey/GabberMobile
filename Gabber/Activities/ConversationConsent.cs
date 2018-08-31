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
        private AppCompatButton submitButton;
        private List<LanguageChoice> languageChoices;
        private Spinner languageSpinner;
        private string selectedLanguage;
        private bool consentChecked;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.consent_conversation);

            SupportActionBar.Title = StringResources.consent_gabber_toolbar_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            ISharedPreferences _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            Project selectedProject = Queries.ProjectById(_prefs.GetInt("SelectedProjectID", 0));

            FindViewById<TextView>(Resource.Id.GabberConsentControlTitle).Text =
                StringResources.consent_gabber_title_control;

            FindViewById<TextView>(Resource.Id.GabberConsentControlDesc).TextFormatted =
                Html.FromHtml(StringResources.consent_gabber_body_control);

            FindViewById<TextView>(Resource.Id.GabberConsentDecisionTitle).Text =
                StringResources.consent_gabber_title_decision;

            FindViewById<TextView>(Resource.Id.GabberConsentDecisionDesc).Text =
                StringResources.consent_gabber_body_decision;

            RadioButton consentTypePublic = FindViewById<RadioButton>(Resource.Id.GabberConsentTypePublic);
            consentTypePublic.Text = StringResources.consent_gabber_consent_type_public_brief;

            FindViewById<TextView>(Resource.Id.GabberConsentTypePublicFull).Text =
                StringResources.consent_gabber_consent_type_public_full;

            RadioButton consentTypeMembers = FindViewById<RadioButton>(Resource.Id.GabberConsentTypeMembers);
            consentTypeMembers.Text = StringResources.consent_gabber_consent_type_members_brief;

            TextView consentTypeMembersFull = FindViewById<TextView>(Resource.Id.GabberConsentTypeMembersFull);
            consentTypeMembersFull.TextFormatted = Html.FromHtml(
                string.Format(StringResources.consent_gabber_consent_type_members_full,
                              selectedProject.Members.Count,
                              selectedProject.Members.FindAll((obj) => obj.Role == "researcher").Count));

            if (selectedProject.IsPublic)
            {
                consentTypeMembers.Visibility = ViewStates.Gone;
                consentTypeMembersFull.Visibility = ViewStates.Gone;
            }

            var consentTypePrivate = FindViewById<RadioButton>(Resource.Id.GabberConsentTypePrivate);
            consentTypePrivate.Text = StringResources.consent_gabber_consent_type_private_brief;

            var consentTypePrivateFull = FindViewById<TextView>(Resource.Id.GabberConsentTypePrivateFull);
            var participants = BuildParticipants(Queries.SelectedParticipants().ToList());
            consentTypePrivateFull.TextFormatted = Html.FromHtml(string.Format(StringResources.consent_gabber_consent_type_private_full, participants));

            var isConsented = FindViewById<RadioGroup>(Resource.Id.GabberConsentProvided);

            submitButton = FindViewById<AppCompatButton>(Resource.Id.GabberConsentSubmit);
            submitButton.Text = StringResources.consent_gabber_submit;
            submitButton.Enabled = false;

            submitButton.Click += (s, e) =>
            {
                // TODO: If (consentTypeMembers.Checked || consentTypePrivate.Checked) 
                // then show dialog to illustrate the affect of this choice.
                var consent = "";
                if (consentTypePublic.Checked) consent = "public";
                if (consentTypeMembers.Checked) consent = "members";
                if (consentTypePrivate.Checked) consent = "private";
                // This is used then deleted when saving the recording session
                _prefs.Edit().PutString("SESSION_CONSENT", consent).Commit();

                LanguageChoice chosenLang = languageChoices.FirstOrDefault((arg) => arg.Endonym == selectedLanguage);

                _prefs.Edit().PutInt("SESSION_LANG", chosenLang.Id).Commit();

                StartActivity(new Intent(this, typeof(ConsentSummary)));
            };

            isConsented.CheckedChange += (s, e) =>
            {
                consentChecked = true;
                CheckIfCanSubmit();
            };

            languageSpinner = FindViewById<Spinner>(Resource.Id.chooseLanguageSpinner);
            languageSpinner.ItemSelected += LanguageSpinner_ItemSelected;

            LoadLanguages();
        }

        private async void LoadLanguages()
        {
            RelativeLayout loadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingLayout);
            loadingLayout.Visibility = ViewStates.Visible;

            languageChoices = await LanguagesManager.GetLanguageChoices();

            if (languageChoices == null || languageChoices.Count == 0)
            {
                new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetTitle(StringResources.common_comms_error)
                    .SetMessage(StringResources.common_comms_error_server)
                    .SetPositiveButton(StringResources.common_comms_retry, (a, b) =>
                    {
                        LoadLanguages();
                    })
                    .SetNegativeButton(StringResources.common_comms_cancel, (a, b) => { Finish(); })
                    .Show();
            }
            else
            {
                loadingLayout.Visibility = ViewStates.Gone;

                List<string> langNames = languageChoices.Select(lang => lang.Endonym).ToList();
                langNames.Insert(0, StringResources.common_ui_forms_language_default);

                LanguageChoice defaultLang = languageChoices.First(lang => lang.Id == Session.ActiveUser.Lang);
                int defaultInd = langNames.IndexOf(defaultLang.Endonym);

                ArrayAdapter spinnerAdapter = new ArrayAdapter(this, Resource.Layout.spinner_row, langNames);
                languageSpinner.Adapter = spinnerAdapter;
                languageSpinner.SetSelection(defaultInd);
            }
        }

        private void CheckIfCanSubmit()
        {
            submitButton.Enabled = (consentChecked &&
                                    !string.IsNullOrWhiteSpace(selectedLanguage) &&
                                    selectedLanguage != StringResources.common_ui_forms_language_default);
        }

        private string BuildParticipants(List<User> participants)
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

        private void LanguageSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedLanguage = (string)languageSpinner.GetItemAtPosition(e.Position);
            CheckIfCanSubmit();
        }

    }
}