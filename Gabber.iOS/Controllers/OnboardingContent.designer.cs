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
    [Register ("OnboardingContent")]
    partial class OnboardingContent
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel OBContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView OBImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton OBLogin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton OBRegister { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel OBTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView OnboardingActions { get; set; }

        [Action ("ShowLogin:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShowLogin (UIKit.UIButton sender);

        [Action ("ShowRegister:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShowRegister (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (OBContent != null) {
                OBContent.Dispose ();
                OBContent = null;
            }

            if (OBImage != null) {
                OBImage.Dispose ();
                OBImage = null;
            }

            if (OBLogin != null) {
                OBLogin.Dispose ();
                OBLogin = null;
            }

            if (OBRegister != null) {
                OBRegister.Dispose ();
                OBRegister = null;
            }

            if (OBTitle != null) {
                OBTitle.Dispose ();
                OBTitle = null;
            }

            if (OnboardingActions != null) {
                OnboardingActions.Dispose ();
                OnboardingActions = null;
            }
        }
    }
}