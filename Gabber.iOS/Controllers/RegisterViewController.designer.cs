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
    [Register ("RegisterViewController")]
    partial class RegisterViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField EmailRegisterTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField FullNameRegisterTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordRegisterTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView RegisterActivityIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView RegisterMasterView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegisterUIButton { get; set; }

        [Action ("Register:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Register (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (EmailRegisterTextField != null) {
                EmailRegisterTextField.Dispose ();
                EmailRegisterTextField = null;
            }

            if (FullNameRegisterTextField != null) {
                FullNameRegisterTextField.Dispose ();
                FullNameRegisterTextField = null;
            }

            if (PasswordRegisterTextField != null) {
                PasswordRegisterTextField.Dispose ();
                PasswordRegisterTextField = null;
            }

            if (RegisterActivityIndicator != null) {
                RegisterActivityIndicator.Dispose ();
                RegisterActivityIndicator = null;
            }

            if (RegisterMasterView != null) {
                RegisterMasterView.Dispose ();
                RegisterMasterView = null;
            }

            if (RegisterUIButton != null) {
                RegisterUIButton.Dispose ();
                RegisterUIButton = null;
            }
        }
    }
}