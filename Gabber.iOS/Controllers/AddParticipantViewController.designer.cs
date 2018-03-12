// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Gabber.iOS
{
    [Register ("AddParticipantViewController")]
    partial class AddParticipantViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AddNewParticipant { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ParticipantEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ParticipantName { get; set; }

        [Action ("AddParticipant:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AddParticipant (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AddNewParticipant != null) {
                AddNewParticipant.Dispose ();
                AddNewParticipant = null;
            }

            if (ParticipantEmail != null) {
                ParticipantEmail.Dispose ();
                ParticipantEmail = null;
            }

            if (ParticipantName != null) {
                ParticipantName.Dispose ();
                ParticipantName = null;
            }
        }
    }
}