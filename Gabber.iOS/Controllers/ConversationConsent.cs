using Foundation;
using Gabber.iOS.ViewSources;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using System;
using System.Collections.Generic;
using UIKit;
using System.Linq;

namespace Gabber.iOS
{
    // This is used to populate the three options on the UI
    public class Consent
    {
        public string Title { get; set; }
        public NSAttributedString Subtitle { get; set; }
    }

    public partial class ConversationConsent : UIViewController
    {
        // The type of consent participants have chosen
        string ConsentType;

        public ConversationConsent (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = StringResources.consent_gabber_toolbar_title;

            int SelectedProjectID = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.IntForKey("SelectedProjectID"));
            var SelectedProject = Queries.ProjectById(SelectedProjectID);

            ConversationConsentTitle.Text = StringResources.consent_gabber_title_control;
            ConversationConsentContent.AttributedText = ResearchConsent.BuildFromHTML(StringResources.consent_gabber_body_control);
            ConversationDecisionTitle.Text = StringResources.consent_gabber_title_decision;
            ConversationDecisionDes.AttributedText = ResearchConsent.BuildFromHTML(StringResources.consent_gabber_body_decision);

            var membersContent = string.Format(
                StringResources.consent_gabber_consent_type_members_full,
                SelectedProject.Members.Count,
                SelectedProject.Members.FindAll((obj) => obj.Role == "researcher").Count);

            var privateContent = string.Format(StringResources.consent_gabber_consent_type_private_full, BuildParticipants());

            var items = new List<Consent> 
            {
                new Consent {
                    Title=StringResources.consent_gabber_consent_type_public_brief,
                    Subtitle=ResearchConsent.BuildFromHTML(StringResources.consent_gabber_consent_type_public_full, 14, false)
                },
                new Consent {
                    Title=StringResources.consent_gabber_consent_type_members_brief,
                    Subtitle=ResearchConsent.BuildFromHTML(membersContent, 14, false)
                },
                new Consent {
                    Title=StringResources.consent_gabber_consent_type_private_brief,
                    Subtitle=ResearchConsent.BuildFromHTML(privateContent, 14, false)
                }
            };

            // Only show members if the project is private
            if (SelectedProject.IsPublic) items.RemoveAt(1);

            var consentVSource = new ConsentViewSource(items);

            consentVSource.ConsentSelected += (int selectedIndex) => 
            {
                var consentOptions = new string[] { "public", "members", "private" };
                ConsentType = consentOptions[selectedIndex];
                ConversationConsentSubmit.Enabled = true;
            };

            ConversationConsentTableView.Source = consentVSource;
            ConversationConsentTableView.RowHeight = UITableView.AutomaticDimension;
            ConversationConsentTableView.EstimatedRowHeight = 86f;

            ConversationConsentSubmit.SetTitle(StringResources.consent_gabber_submit, UIControlState.Normal);
            ConversationConsentSubmit.Layer.BorderWidth = 1.0f;
            ConversationConsentSubmit.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            ConversationConsentSubmit.Enabled = false;

            ConversationConsentSubmit.TouchUpInside += delegate {
                NSUserDefaults.StandardUserDefaults.SetString(ConsentType, "SESSION_CONSENT");
            };
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
        }

        string BuildParticipants()
        {
            var participants = Queries.SelectedParticipants().ToList();
            if (participants.Count == 1) return participants[0].Name.Split(' ')[0];
            var PartNames = new List<string>();
            foreach (var p in participants) PartNames.Add(p.Name.Split(' ')[0].Trim());
            return string.Join(", ", PartNames);
        }
    }
}