using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL.Resources;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class RegisterVerify : UIViewController
    {
        public RegisterVerify(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // The user can navigate back to register (i.e. if they used an incorrect email),
            // and given we have a toolbar a title there rather than content simplifies view.
            Title = StringResources.register_verify_ui_page_title;

            var email = NSUserDefaults.StandardUserDefaults.StringForKey("username");
            RegisterVerifyBody.Text = string.Format(StringResources.register_verify_ui_page_content, email);
            RegisterVerifySubcontent.Text = StringResources.register_verify_ui_page_subcontent;
            RegisterVerifyLogin.SetTitle(StringResources.login_ui_submit_button, UIControlState.Normal);

            RegisterVerifyOpenEmail.Layer.BorderWidth = 1.0f;
            RegisterVerifyOpenEmail.Layer.BorderColor = Application.MainColour;

            RegisterVerifyLogin.Layer.BorderWidth = RegisterVerifyOpenEmail.Layer.BorderWidth;
            RegisterVerifyLogin.Layer.BorderColor = RegisterVerifyOpenEmail.Layer.BorderColor;

            RegisterVerifyLogin.TouchUpInside += delegate
            {
                Logger.LOG_EVENT_WITH_ACTION("LOGIN_BUTTON", "CLICKED");
            };

            RegisterVerifyOpenEmail.SetTitle(StringResources.register_verify_ui_button_openemail, UIControlState.Normal);
            RegisterVerifyOpenEmail.TouchUpInside += delegate
            {
                Logger.LOG_EVENT_WITH_ACTION("EMAIL_CLIENT", "CLICKED");
                UIApplication.SharedApplication.OpenUrl(new NSUrl("message://"));
            };
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "RegisterVerifyToLogin")
            {
                NavigationItem.BackBarButtonItem = new UIBarButtonItem() { Title = "" };
            }
        }
    }
}