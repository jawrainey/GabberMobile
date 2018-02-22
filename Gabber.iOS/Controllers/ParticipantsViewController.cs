using System;
using UIKit;
using Gabber.iOS.ViewSources;
using GabberPCL;
using Foundation;

namespace Gabber.iOS
{
    public partial class ParticipantsViewController : UIViewController
    {
        ParticipantsCollectionViewSource participantsViewSource;

        public ParticipantsViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            participantsViewSource = new ParticipantsCollectionViewSource(Queries.AllParticipants());
            ParticipantsCollectionView.Source = participantsViewSource;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            participantsViewSource.Rows = Queries.AllParticipants();
            ParticipantsCollectionView.ReloadData();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "SegueToRecordGabber" && Queries.SelectedParticipants().Count == 0)
            {
                PresentViewController(
                    new Helpers.MessageDialog().BuildErrorMessageDialog(
                    "Participants not selected", 
                    "At least one participant must be selected"
                ), true, null);
                return;
            }

            // This removes the default title ("Participants") that appears next 
            // to the text on the back button. Only show button without text.
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
        }

        // Revisited this page, i.e. after adding a participant
        [Action("UnwindToParticipantsViewController:")]
        public void UnwindToParticipantsViewController(UIStoryboardSegue segue) {}
    }
}