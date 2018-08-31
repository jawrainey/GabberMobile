using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using Java.Lang;
using static Android.Preferences.Preference;

namespace Gabber.Fragments
{
    public class PrefsFragment : PreferenceFragmentCompat
    {
        private static PrefsFragment instance;
        private List<LanguageChoice> allLangs;

        public static PrefsFragment NewInstance()
        {
            if (instance == null) instance = new PrefsFragment();
            return instance;
        }

        public override async void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);

            allLangs = (await LanguagesManager.GetLanguageChoices()).OrderBy((lang) => lang.Code).ToList();

            string[] langIds = allLangs.Select((lang) => lang.Id.ToString()).ToArray();
            string[] langNames = allLangs.Select((lang) => lang.Endonym).ToArray();

            CultureInfo currentCulture = StringResources.Culture ?? Localise.GetCurrentCultureInfo();

            int appCurrentLangVal = allLangs.FindIndex((obj) => obj.Code == currentCulture.TwoLetterISOLanguageName);
            if (appCurrentLangVal == -1) appCurrentLangVal = 1;

            var appLangPref = (Android.Support.V7.Preferences.ListPreference)FindPreference("appLanguagePref");
            appLangPref.Title = StringResources.settings_chooseAppLanguage;
            appLangPref.SetEntries(langNames);
            appLangPref.SetEntryValues(langIds);
            appLangPref.SetValueIndex(appCurrentLangVal);
            appLangPref.PreferenceChange += AppLangPrefChanged;

            int convoDefaultVal = allLangs.FindIndex((obj) => obj.Id == Session.ActiveUser.Lang);

            var convoLangPref = (Android.Support.V7.Preferences.ListPreference)FindPreference("convoLanguagePref");
            convoLangPref.Title = StringResources.settings_chooseConvoLanguage;
            convoLangPref.SetEntries(langNames);
            convoLangPref.SetEntryValues(langIds);
            convoLangPref.SetValueIndex(convoDefaultVal);
            convoLangPref.PreferenceChange += ConvoLangPrefChanged;

            var logOutPref = FindPreference("logOutPref");
            logOutPref.Title = StringResources.settings_logout;
            logOutPref.PreferenceClick += LogOutPref_PreferenceClick;

        }

        private void AppLangPrefChanged(object sender, Android.Support.V7.Preferences.Preference.PreferenceChangeEventArgs e)
        {
            int newLangVal = -1;

            int.TryParse((string)e.NewValue, out newLangVal);

            LanguageChoice chosen = allLangs.FirstOrDefault((lang) => lang.Id == newLangVal);

            if (newLangVal != -1 && chosen != null && chosen.Code != StringResources.Culture?.TwoLetterISOLanguageName)
            {
                StringResources.Culture = new CultureInfo(chosen.Code);
                ((MainActivity)Activity).LoadStrings();
            }
        }

        private void ConvoLangPrefChanged(object sender, Android.Support.V7.Preferences.Preference.PreferenceChangeEventArgs e)
        {
            int newLangVal = -1;

            int.TryParse((string)e.NewValue, out newLangVal);

            if (newLangVal != -1 && newLangVal != Session.ActiveUser.Lang)
            {
                Session.ActiveUser.Lang = newLangVal;
                Queries.SaveActiveUser();

                //TODO update on server
            }
        }

        private void LogOutPref_PreferenceClick(object sender, Android.Support.V7.Preferences.Preference.PreferenceClickEventArgs e)
        {
            new AlertDialog.Builder(Activity)
                .SetTitle(StringResources.settings_logout_dialog_title)
                .SetMessage(StringResources.settings_logout_dialog_message)
                .SetNegativeButton(StringResources.common_comms_cancel, (a, b) => { })
                .SetPositiveButton(StringResources.settings_logout_dialog_confirm, (a, b) => { ((MainActivity)Activity).LogOut(); })
                .Show();
        }

    }
}
