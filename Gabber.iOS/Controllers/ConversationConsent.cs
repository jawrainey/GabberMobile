using Foundation;
using Gabber.iOS.ViewSources;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using System;
using System.Collections.Generic;
using UIKit;
using System.Linq;
using Gabber.iOS.Helpers;
using GabberPCL.Interfaces;

namespace Gabber.iOS
{
    // This is used to populate the three options on the UI
    public class Consent
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
    }

    public partial class ConversationConsent : UIViewController
    {
        // The type of consent participants have chosen
        private string ConsentType;
        private ProfileOptionPickerViewModel pickerModel;

        public ConversationConsent(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = StringResources.consent_gabber_toolbar_title;

            int SelectedProjectID = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.IntForKey("SelectedProjectID"));
            var SelectedProject = Queries.ProjectById(SelectedProjectID);
            var content = Queries.ContentByLanguage(SelectedProject, Localize.GetCurrentCultureInfo());

            ConversationDecisionDes.Text = string.Format(StringResources.consent_gabber_body_decision, Config.PRINT_URL);
            ChooseLanguageTitle.Text = StringResources.conversation_language_prompt;

            List<User> participants = Queries.SelectedParticipants().ToList();
            string inConversation = StringResources.consent_gabber_consent_type_private_full_you;

            for (int i = 0; i < participants.Count; i++)
            {
                User p = participants[i];
                if (p.Id == Session.ActiveUser.Id) continue;
                inConversation += ", " + p.Name;
            }

            int inProject = SelectedProject.Members.Count;

            var membersContent = string.Format(
                StringResources.consent_gabber_consent_type_members_full,
                inProject, (inProject > 1) ? StringResources.consent_gabber_consent_type_members_full_plural :
                StringResources.consent_gabber_consent_type_members_full_singular);

            var privateContent = string.Format(StringResources.consent_gabber_consent_type_private_full, inConversation);

            var items = new List<Consent>
            {
                new Consent {
                    Title = StringResources.consent_gabber_consent_type_public_brief,
                    Subtitle = StringResources.consent_gabber_consent_type_public_full
                },
                new Consent {
                    Title = string.Format(StringResources.consent_gabber_consent_type_members_brief, content.Title),
                    Subtitle = membersContent
                },
                new Consent {
                    Title=StringResources.consent_gabber_consent_type_private_brief,
                    Subtitle = privateContent
                }
            };

            // Only show members if the project is private
            if (SelectedProject.IsPublic) items.RemoveAt(1);

            var consentVSource = new ConsentViewSource(items);

            consentVSource.ConsentSelected += (int selectedIndex) =>
            {
                var consentOptions = new string[] { "public", "members", "private" };
                ConsentType = consentOptions[selectedIndex];
                CheckSubmitEnabled();
            };

            ConversationConsentTableView.Source = consentVSource;
            ConversationConsentTableView.RowHeight = UITableView.AutomaticDimension;
            ConversationConsentTableView.EstimatedRowHeight = 86f;

            ConversationConsentSubmit.SetTitle(StringResources.consent_gabber_submit, UIControlState.Normal);
            ConversationConsentSubmit.Layer.BorderWidth = 1.0f;
            ConversationConsentSubmit.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            ConversationConsentSubmit.Enabled = false;

            ConversationConsentSubmit.TouchUpInside += delegate
            {
                NSUserDefaults.StandardUserDefaults.SetString(ConsentType, "SESSION_CONSENT");
                NSUserDefaults.StandardUserDefaults.SetInt(pickerModel.GetChoice(LanguagePicker).GetId(), "SESSION_LANG");
            };

            LoadLanguages();
        }

        private async void LoadLanguages()
        {
            List<LanguageChoice> languages = await Localizer.GetLanguageChoices();

            if (languages != null)
            {
                pickerModel = new ProfileOptionPickerViewModel(languages.ToList<IProfileOption>(),
                                                               StringResources.common_ui_forms_language_default,
                                                               PickerSelected);
                LanguagePicker.Model = pickerModel;
                pickerModel.SelectById(LanguagePicker, Session.ActiveUser.Lang);
            }
        }

        private void PickerSelected(IProfileOption choice)
        {
            CheckSubmitEnabled();
        }

        private void CheckSubmitEnabled()
        {
            ConversationConsentSubmit.Enabled = !string.IsNullOrWhiteSpace(ConsentType) &&
                pickerModel.GetChoice(LanguagePicker) != null;
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
        }

        public override void ViewWillAppear(bool animated)
        {
            nfloat tableHeight = 0f;
            int numRows = (int)ConversationConsentTableView.NumberOfRowsInSection(0);

            for (int i = 0; i < numRows; i++)
            {
                tableHeight += 1.2f * ConversationConsentTableView.RectForRowAtIndexPath(NSIndexPath.FromItemSection(i, 0)).Height;
            }

            TableViewHeight.Constant = tableHeight;
            ConversationConsentTableView.UpdateConstraints();
            ConversationConsentTableView.LayoutIfNeeded();
        }
    }
}