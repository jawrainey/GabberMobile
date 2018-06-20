using System;
using System.Collections.Generic;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    public partial class OnboardingContent : UIViewController
    {
        public int Index;
        public string OBCTitle;
        public string OBCContent;

        public OnboardingContent (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var options = new List<string> { "Welcome", "Projects", "Parts", "Welcome", "Consent" };

            OBImage.Image = UIImage.FromBundle($"Onboarding{options[Index]}");
            OBTitle.Text = OBCTitle;
            OBContent.Text = OBCContent;
            // Only show the view on the final screen otherwise space is taken; similar to .GONE on android.
            if (Index < 4) OnboardingActions.RemoveFromSuperview();
            // TODO: animate OnboardingActions when the user is on the final page
            OnboardingActions.Hidden &= Index != 4;

			OBRegister.SetTitle(StringResources.register_ui_submit_button, UIControlState.Normal);
            OBRegister.Layer.BorderWidth = 1.0f;
            OBRegister.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;

			OBLogin.SetTitle(StringResources.login_ui_submit_button, UIControlState.Normal);
            OBLogin.Layer.BorderWidth = OBRegister.Layer.BorderWidth;
            OBLogin.Layer.BorderColor = OBRegister.Layer.BorderColor;
        }

        void ShowView(string type)
        {
            UIViewController controller = null;
            var isLogin = type == "login";
            if (isLogin)
            {
                controller = Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
            }
            else
            {
                controller = Storyboard.InstantiateViewController("RegisterViewController") as RegisterViewController;
            }
            controller.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(
                UIImage.FromBundle("BackButton"),
                UIBarButtonItemStyle.Plain,
                delegate { DismissViewController(true, null); });
            controller.NavigationItem.Title = isLogin ? StringResources.login_ui_title : StringResources.register_ui_title;
            var navBar = new UINavigationController(controller);
            PresentViewController(navBar, true, null);
        }

        partial void ShowRegister(UIButton sender) => ShowView("register");
        partial void ShowLogin(UIButton sender) => ShowView("login");
    }
}