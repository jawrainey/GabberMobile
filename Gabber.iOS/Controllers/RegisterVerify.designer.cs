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
    [Register ("RegisterVerify")]
    partial class RegisterVerify
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RegisterVerifyBody { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegisterVerifyLogin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegisterVerifyOpenEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RegisterVerifySubcontent { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (RegisterVerifyBody != null) {
                RegisterVerifyBody.Dispose ();
                RegisterVerifyBody = null;
            }

            if (RegisterVerifyLogin != null) {
                RegisterVerifyLogin.Dispose ();
                RegisterVerifyLogin = null;
            }

            if (RegisterVerifyOpenEmail != null) {
                RegisterVerifyOpenEmail.Dispose ();
                RegisterVerifyOpenEmail = null;
            }

            if (RegisterVerifySubcontent != null) {
                RegisterVerifySubcontent.Dispose ();
                RegisterVerifySubcontent = null;
            }
        }
    }
}