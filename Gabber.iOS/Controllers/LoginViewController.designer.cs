// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Gabber.iOS
{
    [Register ("LoginViewController")]
    partial class LoginViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField EmailTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LoginErrorLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LoginUIButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordTextField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EmailTextField != null) {
                EmailTextField.Dispose ();
                EmailTextField = null;
            }

            if (LoginErrorLabel != null) {
                LoginErrorLabel.Dispose ();
                LoginErrorLabel = null;
            }

            if (LoginUIButton != null) {
                LoginUIButton.Dispose ();
                LoginUIButton = null;
            }

            if (PasswordTextField != null) {
                PasswordTextField.Dispose ();
                PasswordTextField = null;
            }
        }
    }
}