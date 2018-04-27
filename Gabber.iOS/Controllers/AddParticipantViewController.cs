using System;
using System.Text.RegularExpressions;
using Foundation;
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

        public AddParticipantViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AddNewParticipant.Layer.BorderWidth = 1.0f;
            AddNewParticipant.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;

            NavigationItem.Title = StringResources.participants_ui_dialog_add_title;
            ParticipantName.Placeholder = StringResources.register_ui_fullname_label;
            ParticipantEmail.Placeholder = StringResources.common_ui_forms_email_label;
            AddNewParticipant.SetTitle(StringResources.participants_ui_dialog_add_positive, UIControlState.Normal);

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
                PerformSegue("UnwindToPCV", this);
            }
        }

        void ErrorMessageDialog(string title)
        {
            ErrorDialog.Title = title;
            PresentViewController(ErrorDialog, true, null);
        }
    }
}