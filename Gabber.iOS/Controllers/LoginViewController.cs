using System;
using UIKit;
using GabberPCL;

namespace Gabber.iOS
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle) {}

        async partial void Authenticate(UIButton _)
        {
            // TODO: validate user input
            var client = new RestClient();
            var result = await client.Login(EmailTextField.Text, PasswordTextField.Text);
            // TODO: get active user from db based on email
            Session.ActiveUser.IsActive = true;
            Session.Token = result;
            // TODO: this is obviously buggy ... works for now ...
            UIApplication.SharedApplication.Windows[0].RootViewController = 
                UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
        }
    }
}