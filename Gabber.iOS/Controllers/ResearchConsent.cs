using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL;
using GabberPCL.Resources;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class ResearchConsent : UIViewController
    {
        public ResearchConsent(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = StringResources.consent_research_toolbar_title;
            
            int SelectedProjectID = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.IntForKey("SelectedProjectID"));
            var SelectedProject = Queries.ProjectById(SelectedProjectID);
            var contentOh = Queries.ContentByLanguage(SelectedProject, Localize.GetCurrentCultureInfo());

            var IsOrg = SelectedProject.Organisation.Name.ToLower() == "individual";
            var org = IsOrg ? SelectedProject.Creator.Name : SelectedProject.Organisation.Name;
            var content = string.Format(StringResources.consent_research_body, org, contentOh.Title);

            ResearchConsentDesc.Text = content;
            ResearchConsentFormDetails.Text = StringResources.consent_research_form;
            MoreDetailsButton.SetTitle(StringResources.consent_research_details_button, UIControlState.Normal);

            ResearchConsentSubmit.SetTitle(StringResources.consent_research_submit, UIControlState.Normal);
            ResearchConsentSubmit.Layer.BorderWidth = 1.0f;
            ResearchConsentSubmit.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            ResearchConsentSubmit.Enabled = false;

            MoreDetailsButton.Layer.BorderWidth = ResearchConsentSubmit.Layer.BorderWidth;
            MoreDetailsButton.Layer.BorderColor = ResearchConsentSubmit.Layer.BorderColor;

            MoreDetailsButton.TouchUpInside += (sender, e) =>
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl(Config.ABOUT_DATA_PAGE));
            };

            ResearchConsentFormSwitch.ValueChanged += delegate
            {
                ResearchConsentSubmit.Enabled = ResearchConsentFormSwitch.On;
            };
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
        }
    }
}