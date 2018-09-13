
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddParticipantActivity : AppCompatActivity
    {
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
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register);
            SupportActionBar.Title = StringResources.participants_ui_add_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

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
            submit.Click += Submit_Click;

            TextInputLayout nameInput = FindViewById<TextInputLayout>(Resource.Id.nameLayout);
            nameInput.Hint = StringResources.register_ui_fullname_label;

            TextInputLayout emailInput = FindViewById<TextInputLayout>(Resource.Id.emailLayout);
            emailInput.Hint = StringResources.common_ui_forms_email_label;

            FindViewById<TextInputLayout>(Resource.Id.passwordLayout).Visibility = ViewStates.Gone;
            FindViewById<TextView>(Resource.Id.Terms).Visibility = ViewStates.Gone;

            LoadData();
        }

        private void LoadData()
        {
            socChoices = IFRC_Society.GetOptions();
            genderChoices = Gender.GetOptions();
            ageChoices = AgeRange.GetOptions();
            roleChoices = IFRC_Role.GetOptions();

            List<string> socNames = socChoices.Select(soc => soc.Name).ToList();
            socNames.Insert(0, StringResources.participants_ui_add_society_default);

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

        private void AgeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedAge = (string)ageSpinner.GetItemAtPosition(e.Position);
        }

        private void RoleSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedRole = (string)roleSpinner.GetItemAtPosition(e.Position);
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            AppCompatEditText fname = FindViewById<AppCompatEditText>(Resource.Id.name);
            AppCompatEditText email = FindViewById<AppCompatEditText>(Resource.Id.email);

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
                     selectedSoc == StringResources.participants_ui_add_society_default)
            {
                //National society not selected
                new Android.Support.V7.App.AlertDialog.Builder(this)
                           .SetMessage(StringResources.common_ui_forms_society_error)
                           .SetPositiveButton(StringResources.common_comms_ok, (a, b) => { })
                           .Show();
            }
            else if (string.IsNullOrWhiteSpace(selectedRole) ||
                     selectedRole == StringResources.participants_ui_add_role_default)
            {
                //IFRC role not selected
                new Android.Support.V7.App.AlertDialog.Builder(this)
                           .SetMessage(StringResources.common_ui_forms_role_error)
                           .SetPositiveButton(StringResources.common_comms_ok, (a, b) => { })
                           .Show();
            }
            else if (Queries.UserByEmail(email.Text) != null)
            {
                // User with this email already exists
                new Android.Support.V7.App.AlertDialog.Builder(this)
                           .SetMessage(StringResources.participants_ui_add_email_error)
                           .SetPositiveButton(StringResources.common_comms_ok, (a, b) => { })
                           .Show();
            }
            else
            {
                IFRC_Society chosenSoc = socChoices.FirstOrDefault((arg) => arg.Name == selectedSoc);
                AgeRange chosenAge = ageChoices.FirstOrDefault((arg) => arg.DisplayName == selectedAge);
                IFRC_Role chosenRole = roleChoices.FirstOrDefault((arg) => arg.LocalisedName == selectedRole);
                Gender chosenGender = genderChoices.FirstOrDefault((arg) => arg.LocalisedName == selectedGender);

                if (chosenGender.Enum == Gender.GenderEnum.Custom)
                {
                    chosenGender.Data = customGenderInput.Text;
                }

                var participant = new User
                {
                    Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fname.Text),
                    Email = email.Text,
                    Selected = true,
                    Society = chosenSoc.Id,
                    AgeBracket = (int)chosenAge.Enum,
                    Role = (int)chosenRole.Enum,
                    GenderId = (int)chosenGender.Enum,
                    GenderTerm = chosenGender.Data
                };

                Session.Connection.Insert(participant);

                // Return with the email of the user we just added
                Intent returnIntent = new Intent();
                returnIntent.PutExtra("NEW_EMAIL", email.Text);
                SetResult(Result.Ok, returnIntent);
                Finish();
            }
        }

    }
}
