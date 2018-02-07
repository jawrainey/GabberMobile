using System;
using UIKit;
using GabberPCL;
using Foundation;
using Newtonsoft.Json;

namespace Gabber.iOS
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle) {}

        async partial void Authenticate(UIButton _)
        {
            var email = EmailTextField.Text;
            var passw = PasswordTextField.Text;
            // TODO: validate user input
            var client = new RestClient();
            var result = await client.Login(email, passw);

            // This is used to show ProjectsVC when opening app
            Session.ActiveUser = Queries.FindOrInsertUser(email);
            // It's unclear why I have went down this IsActive route rather than lookup via prefs ...
            Session.ActiveUser.IsActive = true;
            // This is used to simplify REST API access in PCL
            NSUserDefaults.StandardUserDefaults.SetString(JsonConvert.SerializeObject(result), "ActiveUserTokens");
            Session.Token = result;
            NSUserDefaults.StandardUserDefaults.SetString(email, "Username");

            // TODO: this is obviously not ideal, but works for now ...
            UIApplication.SharedApplication.Windows[0].RootViewController = 
                UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
        }
    }
}