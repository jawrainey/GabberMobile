using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Foundation;
using Gabber.iOS.Helpers;
using Gabber.iOS.ViewSources;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    public partial class RegisterViewController : UIViewController
    {
        private LanguagePickerViewModel pickerModel;

        public RegisterViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RegisterUIButton.Layer.BorderWidth = 1.0f;
            RegisterUIButton.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;

            FullNameRegisterTextField.Placeholder = StringResources.register_ui_fullname_label;
            EmailRegisterTextField.Placeholder = StringResources.common_ui_forms_email_label;
            PasswordRegisterTextField.Placeholder = StringResources.common_ui_forms_password_label;
            ConfirmPasswordTextField.Placeholder = StringResources.common_ui_forms_password_confirm_label;
            ChooseLanguagePromptLabel.Text = StringResources.common_ui_forms_language_prompt;

            var termsContent = string.Format(StringResources.register_ui_terms_label, Config.WEB_URL);
            Terms.AttributedText = ResearchConsent.BuildFromHTML(termsContent, 14, false);
            Terms.DataDetectorTypes = UIDataDetectorType.Link;

            RegisterUIButton.SetTitle(StringResources.register_ui_submit_button, UIControlState.Normal);

            FullNameRegisterTextField.ShouldReturn += NavigateNext;
            EmailRegisterTextField.ShouldReturn += NavigateNext;
            PasswordRegisterTextField.ShouldReturn += NavigateNext;
            ConfirmPasswordTextField.ShouldReturn += NavigateNext;

            LoadLanguages();
        }

        private async void LoadLanguages()
        {
            List<LanguageChoice> languages = await LanguagesManager.GetLanguageChoices();

            if (languages != null && languages.Count > 0)
            {
                pickerModel = new LanguagePickerViewModel(languages);
                LanguagePicker.Model = pickerModel;
                LoadingOverlay.Alpha = 0;
                //LoadingOverlay.RemoveFromSuperview();
            }

        }

        bool NavigateNext(UITextField _field)
        {
            if (_field.Tag < 3)
            {
                View.ViewWithTag(_field.Tag + 1).BecomeFirstResponder();
            }
            else
            {
                _field.ResignFirstResponder();
            }
            return false;
        }

        async partial void Register(UIButton sender)
        {
            var fname = FullNameRegisterTextField.Text;
            var email = EmailRegisterTextField.Text;
            var passw = PasswordRegisterTextField.Text;

            // TODO: Should refactor validation to a helper class as this is shared with LoginVC
            if (string.IsNullOrWhiteSpace(fname))
            {
                ErrorMessageDialog(StringResources.register_ui_fullname_validate_empty);
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                ErrorMessageDialog(StringResources.common_ui_forms_email_validate_empty);
            }
            else if (string.IsNullOrWhiteSpace(passw))
            {
                ErrorMessageDialog(StringResources.common_ui_forms_password_validate_empty);
            }
            else if (!Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                ErrorMessageDialog(StringResources.common_ui_forms_email_validate_invalid);
            }
            else if (PasswordRegisterTextField.Text != ConfirmPasswordTextField.Text)
            {
                ErrorMessageDialog(StringResources.common_ui_forms_password_validate_mismatch);
            }
            else if (pickerModel.GetChoice(LanguagePicker) == null)
            {
                ErrorMessageDialog(StringResources.common_ui_forms_language_error);
            }
            else
            {
                ConfirmPasswordTextField.BecomeFirstResponder();
                ConfirmPasswordTextField.ResignFirstResponder();

                RegisterUIButton.Enabled = false;
                Logger.LOG_EVENT_WITH_ACTION("REGISTER", "ATTEMPT");

                LoadingOverlay.Alpha = 1;

                var response = await RestClient.Register(fname, email, passw, pickerModel.GetChoice(LanguagePicker).Id);
                RegisterUIButton.Enabled = true;

                LoadingOverlay.Alpha = 0;

                if (response.Meta.Success)
                {
                    Logger.LOG_EVENT_WITH_ACTION("REGISTER", "SUCCESS");
                    NSUserDefaults.StandardUserDefaults.SetString(email, "username");
                    PerformSegue("ShowVerifySegue", this);
                }
                else if (response.Meta.Messages.Count > 0)
                {
                    Logger.LOG_EVENT_WITH_ACTION("REGISTER", "ERROR");
                    // Note: errors returned by register are the same as logjn, hence using that for lookup.
                    var err = StringResources.ResourceManager.GetString($"login.api.error.{response.Meta.Messages[0]}");
                    ErrorMessageDialog(err);
                }
            }
        }

        void ErrorMessageDialog(string title)
        {
            var dialog = new MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog(title, "");
            PresentViewController(errorDialog, true, null);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "ShowVerifySegue")
            {
                NavigationItem.BackBarButtonItem = new UIBarButtonItem() { Title = "" };
            }
        }
    }
}