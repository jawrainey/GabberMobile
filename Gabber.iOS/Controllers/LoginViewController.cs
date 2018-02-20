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
                var client = new RestClient();
                var tokens = await client.Login(email, passw, (message) => ErrorMessageDialog(message));

                // If the user details are correct: then a token was generated
                if (!string.IsNullOrEmpty(tokens.Access))
                {
                    // This is used to show ProjectsVC when opening app
                    Session.ActiveUser = Queries.FindOrInsertUser(email);
                    // It's unclear why I have went down this IsActive route rather than lookup via prefs ...
                    Session.ActiveUser.IsActive = true;
                    // This is used to simplify REST API access in PCL
                    NSUserDefaults.StandardUserDefaults.SetString(JsonConvert.SerializeObject(tokens), "ActiveUserTokens");
                    Session.Token = tokens;
                    NSUserDefaults.StandardUserDefaults.SetString(email, "Username");

                    // TODO: this is obviously not ideal, but works for now ...
                    UIApplication.SharedApplication.Windows[0].RootViewController =
                        UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
                }
            }
        }

        void ErrorMessageDialog(string message)
        {
            var dialog = new Helpers.MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog(message);
            PresentViewController(errorDialog, true, null);
        }
    }
}