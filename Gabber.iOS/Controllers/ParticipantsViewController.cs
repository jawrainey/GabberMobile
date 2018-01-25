using System;
using UIKit;
using System.Collections.Generic;
using Gabber.iOS.ViewSources;
using GabberPCL;
using Foundation;
using System.Linq;

namespace Gabber.iOS
{
    public partial class ParticipantsViewController : UIViewController
    {
        ParticipantsCollectionViewSource participantsViewSource;

        public ParticipantsViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var _participants = Queries.AllParticipants();

            if (_participants.Count <= 0)
            {
                Session.Connection.Insert(new Participant
                {
                    Name = "You",
                    Email = "TODO_AFTER_LOGIN",
                    Selected = true
                });
            }

            participantsViewSource = new ParticipantsCollectionViewSource(_participants);
            ParticipantsCollectionView.Source = participantsViewSource;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            participantsViewSource.Rows = Queries.AllParticipants();
            ParticipantsCollectionView.ReloadData();
        }

        // Revisited this page, i.e. after adding a participant
        [Action("UnwindToParticipantsViewController:")]
        public void UnwindToParticipantsViewController(UIStoryboardSegue segue) {}
    }
}