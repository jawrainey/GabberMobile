using Foundation;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle)
        {
            
        }

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            return base.ShouldPerformSegue(segueIdentifier, sender);
            if (EmailTextField.Text == "hello" && PasswordTextField.Text == "password") {
                return base.ShouldPerformSegue(segueIdentifier, sender);
            } 
            else 
            {
                LoginErrorLabel.Text = "Something went wrong...";
                return false;    
            }
        }
    }
}