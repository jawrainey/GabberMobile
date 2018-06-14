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

            int SelectedProjectID = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.IntForKey("SelectedProjectID"));
            var SelectedProject = Queries.ProjectById(SelectedProjectID);
            var org = SelectedProject.Organisation == null ? SelectedProject.Creator.Name : SelectedProject.Organisation.Name;
            var content = string.Format(StringResources.consent_research_body, SelectedProject.Title, org);

            ResearchConsentDesc.AttributedText = BuildFromHTML(content);
            ResearchConsentFormDetails.Text = StringResources.consent_research_form;

            ResearchConsentSubmit.SetTitle(StringResources.consent_research_submit, UIControlState.Normal);
            ResearchConsentSubmit.Layer.BorderWidth = 1.0f;
            ResearchConsentSubmit.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            ResearchConsentSubmit.Enabled = false;

            ResearchConsentFormSwitch.ValueChanged += delegate {
                ResearchConsentSubmit.Enabled = ResearchConsentFormSwitch.On;
            };
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            // Identify that the content is scrollable
            ResearchConsentSV.FlashScrollIndicators();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
        }

        public static NSAttributedString BuildFromHTML(string content, int fsize=16, bool justify=true)
        {
            // Style the content
            var _content = $"<span style=\"font-family: .SF UI Text; font-size: {fsize};\">{content}</span>";
            // Convert the HTML in the content string to a NSAttributedString
            var err = new NSError();
            var atts = new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML };
            var html = new NSAttributedString(NSData.FromString(_content), atts, ref err);
            // Now the content is converted to HTML, we want to justify it
            var mutableContent = new NSMutableAttributedString(html);
            var para = new NSMutableParagraphStyle
            {
                Alignment = justify ? UITextAlignment.Justified : UITextAlignment.Left
            };
            mutableContent.AddAttribute(UIStringAttributeKey.ParagraphStyle, para, new NSRange(0, html.Length - 1));
            return mutableContent;
        }
    }
}