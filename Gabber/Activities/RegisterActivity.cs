using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using GabberPCL;
using GabberPCL.Resources;
using Android.Text.Util;
using System.Threading.Tasks;
using System.Collections.Generic;
using GabberPCL.Models;
using System.Linq;

namespace Gabber
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class RegisterActivity : AppCompatActivity
    {
        FirebaseAnalytics firebaseAnalytics;

        private List<LanguageChoice> languageChoices;

        private List<IFRC_Society> socChoices;
        private Spinner socSpinner;
        private string selectedSoc;

        private List<Gender> genderChoices;
        private Spinner genderSpinner;
        private string selectedGender;
        private TextInputLayout customGenderLayout;
        private TextInputEditText customGenderInput;

        private List<IFRC_Role> roleChoices;
        private Spinner roleSpinner;
        private string selectedRole;

        private List<AgeRange> ageChoices;
        private Spinner ageSpinner;
        private string selectedAge;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);

            SupportActionBar.Title = StringResources.register_ui_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            FindViewById<TextView>(Resource.Id.loadingMessage).Text = StringResources.common_comms_loading;

            socSpinner = FindViewById<Spinner>(Resource.Id.chooseSocietySpinner);
            socSpinner.ItemSelected += SocSpinner_ItemSelected;

            genderSpinner = FindViewById<Spinner>(Resource.Id.chooseGenderSpinner);
            genderSpinner.ItemSelected += GenderSpinner_ItemSelected;

            ageSpinner = FindViewById<Spinner>(Resource.Id.chooseAgeSpinner);
            ageSpinner.ItemSelected += AgeSpinner_ItemSelected;

            roleSpinner = FindViewById<Spinner>(Resource.Id.chooseRoleSpinner);
            roleSpinner.ItemSelected += RoleSpinner_ItemSelected;

            customGenderLayout = FindViewById<TextInputLayout>(Resource.Id.customGenderLayout);
            customGenderInput = FindViewById<TextInputEditText>(Resource.Id.customGender);
            customGenderLayout.Hint = StringResources.common_ui_forms_gender_custom_label;

            AppCompatButton submit = FindViewById<AppCompatButton>(Resource.Id.submit);
            submit.Text = StringResources.register_ui_submit_button;

            TextInputLayout nameInput = FindViewById<TextInputLayout>(Resource.Id.nameLayout);
            nameInput.Hint = StringResources.register_ui_fullname_label;

            TextInputLayout emailInput = FindViewById<TextInputLayout>(Resource.Id.emailLayout);
            emailInput.Hint = StringResources.common_ui_forms_email_label;

            TextInputLayout passwordInput = FindViewById<TextInputLayout>(Resource.Id.passwordLayout);
            passwordInput.Hint = StringResources.common_ui_forms_password_label;

            var terms = FindViewById<TextView>(Resource.Id.Terms);
            var termsContent = string.Format(StringResources.register_ui_terms_label, Config.WEB_URL);
            terms.TextFormatted = Android.Text.Html.FromHtml(termsContent);
            terms.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;

            FindViewById<TextInputEditText>(Resource.Id.password).EditorAction += (_, e) =>
            {
                e.Handled = false;
                if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
                {
                    FindViewById<AppCompatButton>(Resource.Id.submit).PerformClick();
                    e.Handled = true;
                }
            };

            submit.Click += Submit_Click;

            LoadData();
        }

        private async void LoadData()
        {
            RelativeLayout loadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingLayout);
            loadingLayout.Visibility = ViewStates.Visible;

            if (socChoices == null || socChoices.Count == 0)
            {
                socChoices = await IFRC_SocietiesManager.GetSocieties();
            }

            genderChoices = Gender.GetOptions();
            ageChoices = AgeRange.GetOptions();
            roleChoices = IFRC_Role.GetOptions();

            if (socChoices == null || socChoices.Count == 0)
            {
                new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetTitle(StringResources.common_comms_error)
                    .SetMessage(StringResources.common_comms_error_server)
                    .SetPositiveButton(StringResources.common_comms_retry, (a, b) =>
                       {
                           LoadData();
                       })
                    .SetNegativeButton(StringResources.common_comms_cancel, (a, b) => { Finish(); })
                    .Show();
            }
            else
            {
                loadingLayout.Visibility = ViewStates.Gone;

                List<string> socNames = socChoices.Select(soc => soc.Name).ToList();
                socNames.Insert(0, StringResources.common_ui_forms_society_default);

                ArrayAdapter socAdapter = new ArrayAdapter(this, Resource.Layout.spinner_row, socNames);
                socSpinner.Adapter = socAdapter;

                List<string> genderNames = genderChoices.Select(gender => gender.LocalisedName).ToList();
                ArrayAdapter genderAdapter = new ArrayAdapter(this, Resource.Layout.spinner_row, genderNames);
                genderSpinner.Adapter = genderAdapter;

                List<string> ageNames = ageChoices.Select(age => age.DisplayName).ToList();
                ArrayAdapter ageAdapter = new ArrayAdapter(this, Resource.Layout.spinner_row, ageNames);
                ageSpinner.Adapter = ageAdapter;

                List<string> roleNames = roleChoices.Select(role => role.LocalisedName).ToList();
                ArrayAdapter roleAdapter = new ArrayAdapter(this, Resource.Layout.spinner_row, roleNames);
                roleSpinner.Adapter = roleAdapter;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            LOG_EVENT_WITH_ACTION("BACK_BUTTON", "PRESSED");
            OnBackPressed();
            return true;
        }

        void MakeError(string errorMessage)
        {
            var email = FindViewById<AppCompatEditText>(Resource.Id.email);
            // Using login string lookup as there are no different error messages between login/register, only general.
            var message = StringResources.ResourceManager.GetString($"login.api.error.{errorMessage}");
            Snackbar.Make(email, message, Snackbar.LengthLong).Show();
        }

        void LOG_EVENT_WITH_ACTION(string eventName, string action)
        {
            var bundle = new Bundle();
            bundle.PutString("ACTION", action);
            bundle.PutString("TIMESTAMP", System.DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            firebaseAnalytics.LogEvent(eventName, bundle);
        }

        private void SocSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedSoc = (string)socSpinner.GetItemAtPosition(e.Position);
        }

        private void GenderSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedGender = (string)genderSpinner.GetItemAtPosition(e.Position);

            if (selectedGender == StringResources.common_ui_forms_gender_custom)
            {
                customGenderLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                customGenderLayout.Visibility = ViewStates.Gone;
                customGenderInput.Text = null;
            }
        }

        private void RoleSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedRole = (string)roleSpinner.GetItemAtPosition(e.Position);
        }

        private void AgeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedAge = (string)ageSpinner.GetItemAtPosition(e.Position);
        }

        private async void Submit_Click(object sender, System.EventArgs e)
        {
            AppCompatEditText fname = FindViewById<AppCompatEditText>(Resource.Id.name);
            AppCompatEditText email = FindViewById<AppCompatEditText>(Resource.Id.email);
            AppCompatEditText passw = FindViewById<AppCompatEditText>(Resource.Id.password);

            if (string.IsNullOrWhiteSpace(fname.Text))
            {
                // No name entered
                fname.Error = StringResources.register_ui_fullname_validate_empty;
                fname.RequestFocus();
            }
            else if (string.IsNullOrWhiteSpace(email.Text))
            {
                // No email entered
                email.Error = StringResources.common_ui_forms_email_validate_empty;
                email.RequestFocus();
            }
            else if (string.IsNullOrWhiteSpace(passw.Text))
            {
                // no password entered
                passw.Error = StringResources.common_ui_forms_password_validate_empty;
                passw.RequestFocus();
            }
            else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
            {
                // Email not valid
                email.Error = StringResources.common_ui_forms_email_validate_invalid;
                email.RequestFocus();
            }
            else if (string.IsNullOrWhiteSpace(selectedGender) ||
                     selectedGender == StringResources.common_ui_forms_gender_default ||
                     (selectedGender == StringResources.common_ui_forms_gender_custom && string.IsNullOrWhiteSpace(customGenderInput.Text)))
            {
                // Gender not selected, OR
                // if custom selected, custom name not entered
                new Android.Support.V7.App.AlertDialog.Builder(this)
                           .SetMessage(StringResources.common_ui_forms_gender_error)
                           .SetPositiveButton(StringResources.common_comms_ok, (a, b) => { })
                           .Show();
            }
            else if (string.IsNullOrWhiteSpace(selectedSoc) ||
                     selectedSoc == StringResources.common_ui_forms_society_default)
            {
                //National society not selected
                new Android.Support.V7.App.AlertDialog.Builder(this)
                           .SetMessage(StringResources.common_ui_forms_society_error)
                           .SetPositiveButton(StringResources.common_comms_ok, (a, b) => { })
                           .Show();
            }
            else if (string.IsNullOrWhiteSpace(selectedRole) ||
                     selectedRole == StringResources.common_ui_forms_role_default)
            {
                //IFRC role not selected
                new Android.Support.V7.App.AlertDialog.Builder(this)
                           .SetMessage(StringResources.common_ui_forms_role_error)
                           .SetPositiveButton(StringResources.common_comms_ok, (a, b) => { })
                           .Show();
            }
            else
            {
                FindViewById<RelativeLayout>(Resource.Id.loadingLayout).Visibility = ViewStates.Visible;
                FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = false;

                IFRC_Society chosenSoc = socChoices.FirstOrDefault((arg) => arg.Name == selectedSoc);
                AgeRange chosenAge = ageChoices.FirstOrDefault((arg) => arg.DisplayName == selectedAge);
                IFRC_Role chosenRole = roleChoices.FirstOrDefault((arg) => arg.LocalisedName == selectedRole);
                Gender chosenGender = genderChoices.FirstOrDefault((arg) => arg.LocalisedName == selectedGender);

                if (chosenGender.Enum == Gender.GenderEnum.Custom)
                {
                    chosenGender.Data = customGenderInput.Text;
                }

                LOG_EVENT_WITH_ACTION("REGISTER", "ATTEMPT");

                //default to English at registration
                CustomAuthResponse response = await RestClient.Register(fname.Text, email.Text.ToLower(), passw.Text, 1, chosenSoc.Id, chosenGender, (int)chosenRole.Enum, (int)chosenAge.Enum);

                if (response.Meta.Success)
                {
                    LOG_EVENT_WITH_ACTION("REGISTER", "SUCCESS");
                    var intent = new Intent(this, typeof(Activities.RegisterVerification));
                    intent.PutExtra("EMAIL_USED_TO_REGISTER", email.Text);
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(intent);
                    Finish();
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        if (response.Meta.Messages.Count > 0)
                        {
                            LOG_EVENT_WITH_ACTION("REGISTER", "ERROR");
                            response.Meta.Messages.ForEach(MakeError);
                        }
                        FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = true;
                        FindViewById<RelativeLayout>(Resource.Id.loadingLayout).Visibility = ViewStates.Gone;
                        fname.RequestFocus();
                    });
                }
            }
        }

    }
}