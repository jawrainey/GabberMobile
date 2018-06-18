using Foundation;
using System;
using UIKit;
using GabberPCL;
using System.Collections.Generic;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.iOS
{
    public partial class ConsentSummary : UIViewController
    {
        public ConsentSummary (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = StringResources.consent_summary_title;

            ConsentSummaryTitle.Text = StringResources.consent_summary_subtitle;

            var SelectedProjectID = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.IntForKey("SelectedProjectID"));
            var SelectedProject = Queries.ProjectById(SelectedProjectID);
            var consentType = NSUserDefaults.StandardUserDefaults.StringForKey("SESSION_CONSENT");

            ConsentTitleDesc.AttributedText = BuildHTML(StringResources.consent_summary_screen_content_project_title, SelectedProject.Title);
            var participantsDesc = string.Format(StringResources.consent_summary_screen_content_participants_desc, BuildParticipants(Queries.SelectedParticipants()));
            ConsentParticipantsDesc.AttributedText = BuildHTML(StringResources.consent_summary_screen_content_participants_title, participantsDesc);
            ConsentResearchDesc.AttributedText = BuildHTML(StringResources.consent_summary_screen_content_research_title, StringResources.consent_summary_screen_content_research_desc);
            ConsentConversationDesc.AttributedText = BuildHTML(StringResources.consent_summary_screen_content_conversation_title, string.Format(StringResources.consent_summary_screen_content_conversation_desc, consentType));
            ConsentEmbargoDesc.AttributedText = BuildHTML(StringResources.consent_summary_screen_content_embargo_title, StringResources.consent_summary_screen_content_embargo_desc);

            ConsentSummarySubmit.SetTitle(StringResources.consent_summary_screen_action, UIControlState.Normal);
            ConsentSummarySubmit.Layer.BorderWidth = 1.0f;
            ConsentSummarySubmit.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
        }

        NSAttributedString BuildHTML(string title, string content) => ResearchConsent.BuildFromHTML($"&bullet; <b>{title}:</b> {content}", 14, false);

        string BuildParticipants(List<User> participants)
        {
            if (participants.Count == 1) return participants[0].Name.Split(' ')[0];
            var PartNames = new List<string>();
            foreach (var p in participants) PartNames.Add(p.Name.Split(' ')[0].Trim());

            return string.Join(", ", PartNames);
        }
    }
}
