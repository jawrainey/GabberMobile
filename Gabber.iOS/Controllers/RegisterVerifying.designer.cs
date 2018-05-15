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
    [Register ("RegisterVerifying")]
    partial class RegisterVerifying
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel VerifyContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton VerifyLoginButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView VerifySpinner { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel VerifyTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (VerifyContent != null) {
                VerifyContent.Dispose ();
                VerifyContent = null;
            }

            if (VerifyLoginButton != null) {
                VerifyLoginButton.Dispose ();
                VerifyLoginButton = null;
            }

            if (VerifySpinner != null) {
                VerifySpinner.Dispose ();
                VerifySpinner = null;
            }

            if (VerifyTitle != null) {
                VerifyTitle.Dispose ();
                VerifyTitle = null;
            }
        }
    }
}