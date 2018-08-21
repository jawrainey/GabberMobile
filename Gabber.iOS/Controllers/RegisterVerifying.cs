using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL;
using GabberPCL.Resources;
using Newtonsoft.Json;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class RegisterVerifying : UIViewController
    {
        public RegisterVerifying(IntPtr handle) : base(handle) { }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            VerifyTitle.Text = StringResources.register_verifying_ui_page_title;
            VerifyContent.Text = StringResources.register_verifying_ui_page_content;
            VerifyLoginButton.SetTitle(StringResources.login_ui_submit_button, UIControlState.Normal);

            VerifyLoginButton.Layer.BorderWidth = 1.0f;
            VerifyLoginButton.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;

            VerifyLoginButton.TouchUpInside += delegate
            {
                Logger.LOG_EVENT_WITH_ACTION("EMAIL_VERIFICATION", "LOGIN_CLICKED");
            };

            var url = NSUserDefaults.StandardUserDefaults.URLForKey("VERIFY_URL");

            var response = await RestClient.RegisterVerify(url.LastPathComponent);

            if (response.Meta.Success)
            {
                Logger.LOG_EVENT_WITH_ACTION("EMAIL_VERIFICATION", "SUCCESS");
                NSUserDefaults.StandardUserDefaults.SetString(JsonConvert.SerializeObject(response.Data.Tokens), "tokens");
                NSUserDefaults.StandardUserDefaults.SetString(response.Data.User.Email, "username");
                Queries.SetActiveUser(response.Data);

                UIApplication.SharedApplication.Windows[0].RootViewController =
                    UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
            }
            else
            {
                Logger.LOG_EVENT_WITH_ACTION("EMAIL_VERIFICATION", "ERROR");
                VerifyContent.Text = string.Format("{0} ", StringResources.register_verifying_ui_page_content_error);
                // Saves popping a dialog
                VerifyContent.Text += StringResources.ResourceManager.GetString($"register.verifying.api.error.{response.Meta.Messages[0]}");
                VerifySpinner.Hidden = true;
                VerifyLoginButton.Hidden = false;
            }
        }
    }
}