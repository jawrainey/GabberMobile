using System;
using System.Text.RegularExpressions;
using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    public partial class AddParticipantViewController : UIViewController
    {
        // Only want to create one Dialog to reuse it.
        UIAlertController ErrorDialog;

        public AddParticipantViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AddNewParticipant.Layer.BorderWidth = 1.0f;
            AddNewParticipant.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;

            NavigationItem.Title = StringResources.participants_ui_add_title;
            ParticipantName.Placeholder = StringResources.register_ui_fullname_label;
            ParticipantEmail.Placeholder = StringResources.common_ui_forms_email_label;
            AddNewParticipant.SetTitle(StringResources.participants_ui_add_positive, UIControlState.Normal);

            ParticipantName.ShouldReturn += NavigateNext;
            ParticipantEmail.ShouldReturn += NavigateNext;

            ParticipantName.BecomeFirstResponder();

            ErrorDialog = new Helpers.MessageDialog().BuildErrorMessageDialog("", "");
        }

        bool NavigateNext(UITextField _field)
        {
            if (_field.Tag == 0)
            {
                View.ViewWithTag(1).BecomeFirstResponder();
            }
            else
            {
                _field.ResignFirstResponder();
                AddParticipant(AddNewParticipant);
            }
            return false;
        }

        partial void AddParticipant(UIButton sender)
        {
            var name = ParticipantName.Text;
            var email = ParticipantEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ErrorMessageDialog(StringResources.register_ui_fullname_validate_empty);
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                ErrorMessageDialog(StringResources.common_ui_forms_email_validate_empty);
            }
            else if (!Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                ErrorMessageDialog(StringResources.common_ui_forms_email_validate_invalid);
            }
            else
            {
                Session.Connection.Insert(new User
                {
                    Name = name,
                    Email = email,
                    Selected = true
                });
                LOG_ADD_PARTICIPANT(name, email);
                PerformSegue("UnwindToPCV", this);
            }
        }

        void LOG_ADD_PARTICIPANT(string name, string email)
        {
            // Logging information
            NSString[] keys = {
                new NSString("NAME"),
                new NSString("EMAIL"),
                new NSString("TIMESTAMP")
            };
            NSObject[] values = {
                new NSString(name),
                new NSString(email),
                new NSString(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys, keys.Length);
            Logger.LOG_EVENT_WITH_DICT("ADD_PARTICIPANT", parameters);
        }

        void ErrorMessageDialog(string title)
        {
            ErrorDialog.Title = title;
            PresentViewController(ErrorDialog, true, null);
        }
    }
}