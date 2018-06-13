using CoreGraphics;
using Foundation;
using GabberPCL;
using GabberPCL.Resources;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class ConversationConsent : UIViewController
    {
        public ConversationConsent (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = StringResources.consent_gabber_toolbar_title;

            ConversationConsentTitle.Text = StringResources.consent_gabber_title_control;
            ConversationConsentContent.AttributedText = BuildDescFromHTML();
            ConversationConsentContent.Text = "";
            ConversationDecisionTitle.Text = StringResources.consent_gabber_title_decision;
            ConversationDecisionDes.Text = StringResources.consent_gabber_body_decision;

            ConversationConsentSubmit.SetTitle(StringResources.conversation_consent_gabber_submit, UIControlState.Normal);
            ConversationConsentSubmit.Layer.BorderWidth = 1.0f;
            ConversationConsentSubmit.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            // ConversationConsentSubmit.Enabled = false;

            ConversationConsentSubmit.TouchUpInside += delegate {
                // TODO: check which one is selected and store it in consent below
                var consent = "members";
                NSUserDefaults.StandardUserDefaults.SetString(consent, "SESSION_CONSENT");
            };
        }

        NSAttributedString BuildDescFromHTML()
        {
            var content = $"<span style=\"font-family: .SF UI Text; font-size: 17;\">{StringResources.consent_gabber_body_control}</span>";
            var err = new NSError();
            var atts = new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML };
            return new NSAttributedString(NSData.FromString(content), atts, ref err);
        }
    }
}