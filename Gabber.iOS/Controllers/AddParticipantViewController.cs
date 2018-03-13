using System;
using System.Text.RegularExpressions;
using Foundation;
using GabberPCL;
using GabberPCL.Models;
using UIKit;

namespace Gabber.iOS
{
    public partial class AddParticipantViewController : UIViewController
    {
        public AddParticipantViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddNewParticipant.Layer.BorderWidth = .5f;
            AddNewParticipant.Layer.BorderColor = UIColor.Black.CGColor;

            ParticipantName.ShouldReturn += NavigateNext;
            ParticipantEmail.ShouldReturn += NavigateNext;
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
            if (ValidateForm())
            {
                Session.Connection.Insert(new User
                {
                    Name = ParticipantName.Text,
                    Email = ParticipantEmail.Text,
                    Selected = true
                });   
                PerformSegue("UnwindToParticipantsViewController", this);
            }
        }

        public bool ValidateForm()
        {
			var name = ParticipantName.Text;
            var email = ParticipantEmail.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                ErrorMessageDialog("The participants full name is empty");
                return false;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                ErrorMessageDialog("The participants email is empty");
                return false;
            }
            if (!Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                ErrorMessageDialog("The email address provided is invalid");
                return false;
            }
            return true;
        }

        void ErrorMessageDialog(string message)
        {
            var dialog = new Helpers.MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog("Error adding participant", message);
            PresentViewController(errorDialog, true, null);
        }
    }
}