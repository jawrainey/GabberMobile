using System;
using UIKit;
using GabberPCL;
using Foundation;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using GabberPCL.Resources;

namespace Gabber.iOS
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = StringResources.login_ui_title;

            LoginUIButton.Layer.BorderWidth = 1.0f;
            LoginUIButton.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;

            LoginUIButton.SetTitle(StringResources.login_ui_submit_button, UIControlState.Normal);
            EmailTextField.Placeholder = StringResources.common_ui_forms_email_label;
            PasswordTextField.Placeholder = StringResources.common_ui_forms_password_label;

            EmailTextField.ShouldReturn += NavigateNext;
            PasswordTextField.ShouldReturn += NavigateNext;
        }

		bool NavigateNext(UITextField _field)
        {
            // If 'Next' on the email field is pressed, then
            // make the password field the focus, otherwise 
            // the 'Go' button was pressed, so authenticate.
            if (_field.Tag == 0)
            {
                View.ViewWithTag(1).BecomeFirstResponder();
            }
            else
            {
                _field.ResignFirstResponder();
                Authenticate(LoginUIButton);
            }
            return false;
        }

        async partial void Authenticate(UIButton _)
        {
            var email = EmailTextField.Text;
            var passw = PasswordTextField.Text;

            if (string.IsNullOrWhiteSpace(email))
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
                LoginUIButton.Enabled = false;
                var client = new RestClient();
                LoginActivityIndicator.StartAnimating();
                var response = await client.Login(email, passw);
                LoginActivityIndicator.StopAnimating();
                LoginUIButton.Enabled = true;

                if (response.Meta.Messages.Count > 0)
                {
                    // Only show the first error as there 
                    var err = StringResources.ResourceManager.GetString($"login.api.error.{response.Meta.Messages[0]}");
                    ErrorMessageDialog(err);
                }
                else if (!string.IsNullOrEmpty(response.Data?.Tokens.Access))
                {
                    NSUserDefaults.StandardUserDefaults.SetString(JsonConvert.SerializeObject(response.Data.Tokens), "tokens");
                    NSUserDefaults.StandardUserDefaults.SetString(email, "username");

                    Queries.SetActiveUser(response.Data);

                    UIApplication.SharedApplication.Windows[0].RootViewController =
                        UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
                }
            }
        }

        void ErrorMessageDialog(string title)
        {
            var dialog = new Helpers.MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog(title, "");
            PresentViewController(errorDialog, true, null);
        }
    }
}