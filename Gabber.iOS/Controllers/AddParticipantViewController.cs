using System;
using GabberPCL;
using GabberPCL.Models;
using UIKit;

namespace Gabber.iOS
{
    public partial class AddParticipantViewController : UIViewController
    {
        public AddParticipantViewController (IntPtr handle) : base (handle) {}

        partial void AddParticipant(UIButton sender)
        {
            // TODO: input validation
            Session.Connection.Insert(new User {
                Name = ParticipantName.Text,
                Email = ParticipantEmail.Text,
                Selected = true
            });
        }
    }
}