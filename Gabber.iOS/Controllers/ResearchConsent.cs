using Foundation;
using GabberPCL;
using GabberPCL.Resources;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class ResearchConsent : UIViewController
    {
        public ResearchConsent (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = StringResources.consent_research_toolbar_title;

            ResearchConsentTitle.Text = StringResources.consent_research_title;
            ResearchConsentDesc.AttributedText = BuildDescFromHTML();
            ResearchConsentFormDetails.Text = StringResources.consent_research_form;

            ResearchConsentSubmit.SetTitle(StringResources.consent_research_submit, UIControlState.Normal);
            ResearchConsentSubmit.Layer.BorderWidth = 1.0f;
            ResearchConsentSubmit.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            ResearchConsentSubmit.Enabled = false;

            ResearchConsentFormSwitch.ValueChanged += delegate {
                ResearchConsentSubmit.Enabled = ResearchConsentFormSwitch.On;
            };
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
        }

        NSAttributedString BuildDescFromHTML()
        {
            int SelectedProjectID = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.IntForKey("SelectedProjectID"));
            var SelectedProject = Queries.ProjectById(SelectedProjectID);
            var org = SelectedProject.Organisation == null ? SelectedProject.Creator.Name : SelectedProject.Organisation.Name;
            var content = $"<span style=\"font-family: .SF UI Text; font-size: 17;\">{string.Format(StringResources.consent_research_body, SelectedProject.Title, org)}</span>";
            var err = new NSError();
            var atts = new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML };
            return new NSAttributedString(NSData.FromString(content), atts, ref err);
        }
    }
}