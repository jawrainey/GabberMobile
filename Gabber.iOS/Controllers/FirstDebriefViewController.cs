// This file has been autogenerated from a class added in the UI designer.

using System;
using Foundation;
using GabberPCL;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    public partial class FirstDebriefViewController : UIViewController
    {
        public FirstDebriefViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = StringResources.debriefing_activity_title;

            CongratsTitle.Text = StringResources.debriefing_congrats_title;
            CongratsBody.Text = string.Format(StringResources.debriefing_congrats_body, Config.PRINT_URL);

            ConsentTitle.Text = StringResources.debriefing_consent_title;
            ConsentBody1.Text = StringResources.debriefing_consent_body1;
            ConsentBody2.Text = StringResources.debriefing_consent_body2;
            ConsentBody3.Text = StringResources.debriefing_consent_body3;

            FinishedButton.SetTitle(StringResources.debriefing_finish_button, UIControlState.Normal);
            FinishedButton.Layer.BorderWidth = 1.0f;
            FinishedButton.Layer.BorderColor = Application.MainColour;

            FinishedButton.TouchUpInside += FinishedButton_TouchUpInside;

            // Removes the title from the previous (Sessions) view controller as it is too long.
            NavigationController.NavigationBar.TopItem.Title = "";
        }

        void FinishedButton_TouchUpInside(object sender, EventArgs e)
        {
            var prefs = NSUserDefaults.StandardUserDefaults;
            prefs.SetBool(true, "SHOWN_FIRSTUPLOAD");

            NavigationController.PopViewController(true);
        }

    }
}
