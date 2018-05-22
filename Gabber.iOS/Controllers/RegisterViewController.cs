using System;
using System.Text.RegularExpressions;
using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    public partial class RegisterViewController : UIViewController
    {
        public RegisterViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RegisterUIButton.Layer.BorderWidth = 1.0f;
            RegisterUIButton.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;

            FullNameRegisterTextField.Placeholder = StringResources.register_ui_fullname_label;
            EmailRegisterTextField.Placeholder = StringResources.common_ui_forms_email_label;
            PasswordRegisterTextField.Placeholder = StringResources.common_ui_forms_password_label;
            RegisterUIButton.SetTitle(StringResources.register_ui_submit_button, UIControlState.Normal);

            FullNameRegisterTextField.ShouldReturn += NavigateNext;
            EmailRegisterTextField.ShouldReturn += NavigateNext;
            PasswordRegisterTextField.ShouldReturn += NavigateNext;
        }

        bool NavigateNext(UITextField _field)
        {
            if (_field.Tag == 0)
            {
                View.ViewWithTag(1).BecomeFirstResponder();
            }
            else if (_field.Tag == 1) 
            {
                View.ViewWithTag(2).BecomeFirstResponder();
            }
            else
            {
                Register(RegisterUIButton);
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
            else
            {
				PasswordRegisterTextField.BecomeFirstResponder();
				PasswordRegisterTextField.ResignFirstResponder();

                RegisterUIButton.Enabled = false;
                var client = new RestClient();
                RegisterActivityIndicator.StartAnimating();
                Logger.LOG_EVENT_WITH_ACTION("REGISTER", "ATTEMPT");
                var response = await client.Register(fname, email, passw);
                RegisterActivityIndicator.StopAnimating();
                RegisterUIButton.Enabled = true;

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