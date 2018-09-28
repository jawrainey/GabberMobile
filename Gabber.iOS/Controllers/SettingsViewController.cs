using System;
using GabberPCL.Resources;
using SafariServices;
using UIKit;
using Gabber.iOS.ViewSources;
using System.Collections.Generic;
using GabberPCL.Models;
using GabberPCL;
using System.Linq;
using System.Globalization;
using Gabber.iOS.Helpers;
using Foundation;
using System.IO;
using GabberPCL.Interfaces;

namespace Gabber.iOS
{
    public class SettingsCell
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
    }

    public partial class SettingsViewController : UIViewController
    {
        void HandleAction(IProfileOption obj)
        {
        }


        List<LanguageChoice> SupportedLanguages;

        int CurrentSelectedPrefLanguageID;
        int CurrentAppLanguageID;

        public SettingsViewController(IntPtr handle) : base(handle) { }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SupportedLanguages = (await LanguageChoiceManager.GetLanguageChoices()).OrderBy((lang) => lang.Code).ToList();
            Title = StringResources.common_menu_settings;
            TabBarController.Title = StringResources.common_menu_settings;
            SettingsTableView.Source = new SettingsTableViewSource(ReCreateSettings(), this, RowSelected);
            // Required to show the data ...
            SettingsTableView.ReloadData();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            TabBarController.Title = StringResources.common_menu_settings;
        }

        void RowSelected(int index)
        {
            switch (index)
            {
                case 0:
                    PresentViewControllerAsync(new SFSafariViewController(new NSUrl(Config.ABOUT_URL)), true);
                    break;
                case 1:
                    UpdateCurrentLanguage();
                    break;
                case 2:
                    UpdatePreferredConversationLanguage();
                    break;
                case 3:
                    Logout();
                    break;
            }
        }

        void Logout()
        {
            var logoutDialog = UIAlertController.Create(
                StringResources.settings_logout_dialog_title,
                StringResources.settings_logout_dialog_message,
                UIAlertControllerStyle.Alert);

            logoutDialog.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (_) => { }));
            logoutDialog.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (_) =>
            {
                // Remove local preferences. This is critical as AppDelegate show onboarding if no token preferences exist.
                NSUserDefaults.StandardUserDefaults.RemovePersistentDomain(NSBundle.MainBundle.BundleIdentifier);
                Session.NukeItFromOrbit();
                NavigationController.PopToRootViewController(false);
                // Go back to onboarding. Next time app is open AppDelegate will take care of showing onboarding.
                var _onboarding = UIStoryboard.FromName("Main", null).InstantiateViewController("Onboarding");
                UIApplication.SharedApplication.Windows[0].RootViewController = _onboarding;
            }));
            PresentViewController(logoutDialog, true, null);
        }

        string CurrentLanguage()
        {
            var currentCulture = StringResources.Culture ?? Localize.GetCurrentCultureInfo();
            int currentLanguage = SupportedLanguages.FindIndex((lang) => lang.Code == currentCulture.TwoLetterISOLanguageName);
            // The language of the device may be in one not supported, e.g. Norsk.
            if (currentLanguage == -1) currentLanguage = SupportedLanguages.FindIndex((lang) => lang.Code == "en");
            return SupportedLanguages[currentLanguage].Endonym;
        }

        void UpdateCurrentLanguage()
        {
            var showLanguagePicker = UIAlertController.Create(
                StringResources.settings_chooseAppLanguage,
                "\n\n\n\n\n\n",
                UIAlertControllerStyle.Alert);

            var pickerModel = new ProfileOptionPickerViewModel(
                SupportedLanguages.ToList<IProfileOption>(),
                StringResources.common_ui_forms_language_default,
                (lang) =>
            {
                if (lang == null || lang.GetId() == 0) return;
                CurrentAppLanguageID = lang.GetId();
            });

            var picker = new UIPickerView { Model = pickerModel, Frame = new CoreGraphics.CGRect(0, 20, 250, 140) };
            pickerModel.SelectById(picker, Session.ActiveUser.AppLang);

            showLanguagePicker.View.AddSubview(picker);

            showLanguagePicker.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (_) => { }));
            showLanguagePicker.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, SaveAppLanguageSelectedFromPicker));
            PresentViewController(showLanguagePicker, true, null);
        }

        void SaveAppLanguageSelectedFromPicker(UIAlertAction _)
        {
            // Because the first element is the description
            if (CurrentAppLanguageID == 0) return;
            var chosen = SupportedLanguages.FirstOrDefault((lang) => lang.Id == CurrentAppLanguageID);
            StringResources.Culture = new CultureInfo(chosen.Code);
            Localize.SetLayoutDirectionByPreference();
            Session.ActiveUser.AppLang = chosen.Id;
            Queries.SaveActiveUser();
            SetTabBarTitles();
            // This is required to update the settings strings to the new language.
            SettingsTableView.Source = new SettingsTableViewSource(ReCreateSettings(), this, RowSelected);
            // Required to update settings strings
            SettingsTableView.ReloadData();
        }

        string PreferredConversationLanguage()
        {
            int preferredLang = SupportedLanguages.FindIndex((lang) => lang.Id == Session.ActiveUser.Lang);
            return SupportedLanguages[preferredLang].Endonym;
        }

        void UpdatePreferredConversationLanguage()
        {
            var showLanguagePicker = UIAlertController.Create(
                StringResources.settings_chooseConvoLanguage,
                "\n\n\n\n\n\n",
                UIAlertControllerStyle.Alert);

            var pickerModel = new ProfileOptionPickerViewModel(
                SupportedLanguages.ToList<IProfileOption>(),
                StringResources.common_ui_forms_language_default,
                (lang) =>
            {
                if (lang == null || lang.GetId() == 0) return;
                CurrentSelectedPrefLanguageID = lang.GetId();
            });

            var picker = new UIPickerView { Model = pickerModel, Frame = new CoreGraphics.CGRect(0, 20, 250, 140) };
            pickerModel.SelectById(picker, Session.ActiveUser.Lang);

            showLanguagePicker.View.AddSubview(picker);

            showLanguagePicker.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (_) => { }));
            showLanguagePicker.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, SaveLanguageSelectedFromPicker));

            PresentViewController(showLanguagePicker, true, null);
        }

        void SaveLanguageSelectedFromPicker(UIAlertAction _)
        {
            Session.ActiveUser.Lang = CurrentSelectedPrefLanguageID;
            Queries.SaveActiveUser();
            SettingsTableView.Source = new SettingsTableViewSource(ReCreateSettings(), this, RowSelected);
            InvokeOnMainThread(SettingsTableView.ReloadData);
            // Push this change, so when they visit the site we can use their preference.
            var suppress = RestClient.PushUpdateForCurrentUser();
        }

        void SetTabBarTitles()
        {
            TabBarController.TabBar.Items[0].Title = StringResources.common_menu_projects;
            TabBarController.TabBar.Items[1].Title = StringResources.common_menu_gabbers;
            TabBarController.TabBar.Items[2].Title = StringResources.common_menu_settings;

            Title = StringResources.common_menu_settings;
            TabBarController.Title = StringResources.common_menu_settings;
        }

        List<SettingsCell> ReCreateSettings()
        {
            return new List<SettingsCell> {
                new SettingsCell { Title=StringResources.settings_about, Subtitle=""},
                new SettingsCell { Title=StringResources.settings_chooseAppLanguage, Subtitle=CurrentLanguage() },
                new SettingsCell { Title=StringResources.settings_chooseConvoLanguage, Subtitle=PreferredConversationLanguage()},
                new SettingsCell { Title=StringResources.settings_logout, Subtitle=""}
            };
        }

    }
}