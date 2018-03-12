using System;
using UIKit;
using GabberPCL;
using Foundation;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Gabber.iOS
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            LoginUIButton.Layer.BorderWidth = .5f;
            LoginUIButton.Layer.BorderColor = UIColor.Black.CGColor;

            EmailTextField.ShouldReturn += NavigateNext;
            PasswordTextField.ShouldReturn += NavigateNext;
            // Move container when keyboard is shown/hidden
            RegisterViewController.SetUpViewKeyboardAnimation(LoginMasterView, LoginMasterView.Bounds);
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
                Authenticate(RegisterUIButton);
            }
            return false;
        }

        async partial void Authenticate(UIButton _)
        {
            var email = EmailTextField.Text;
            var passw = PasswordTextField.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passw))
            {
                ErrorMessageDialog("The username or password is empty");
            }
            else if (!Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                ErrorMessageDialog("The email address provided is invalid");
            }
            else 
            {
                LoginUIButton.Enabled = false;
                var client = new RestClient();
                LoginActivityIndicator.StartAnimating();
                var tokens = await client.Login(email, passw, (message) => ErrorMessageDialog(message));
                LoginActivityIndicator.StopAnimating();
                LoginUIButton.Enabled = true;

                if (!string.IsNullOrEmpty(tokens.Access))
                {
                    NSUserDefaults.StandardUserDefaults.SetString(JsonConvert.SerializeObject(tokens), "ActiveUserTokens");
                    NSUserDefaults.StandardUserDefaults.SetString(email, "Username");

                    UIApplication.SharedApplication.Windows[0].RootViewController =
                        UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
                }
            }
        }

        void ErrorMessageDialog(string message)
        {
            var dialog = new Helpers.MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog("Unable to log in", message);
            PresentViewController(errorDialog, true, null);
        }
    }
}