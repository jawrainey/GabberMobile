using Foundation;
using GabberPCL.Resources;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class RegisterVerify : UIViewController
    {
        public RegisterVerify (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RegisterVerifyTitle.Text = StringResources.register_verify_ui_page_title;
            var email = NSUserDefaults.StandardUserDefaults.StringForKey("username");
            RegisterVerifyBody.Text = string.Format(StringResources.register_verify_ui_page_content, email);
            RegisterVerifySubcontent.Text = StringResources.register_verify_ui_page_subcontent;
            RegisterVerifyLogin.SetTitle(StringResources.login_ui_submit_button, UIControlState.Normal);

            RegisterVerifyOpenEmail.SetTitle(StringResources.register_verify_ui_button_openemail, UIControlState.Normal);
            RegisterVerifyOpenEmail.TouchUpInside += delegate {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("message://"));
            };
        }
    }
}