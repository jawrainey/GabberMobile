using System;
using UIKit;
using System.Collections.Generic;
using Gabber.iOS.ViewSources;

namespace Gabber.iOS
{
    public partial class ParticipantsViewController : UIViewController
    {
        // TODO: should be from PCL
        List<string> participants;

        public ParticipantsViewController (IntPtr handle) : base (handle) 
        {
            participants = new List<string> { "Participant one", "Participant two" };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // TODO: populate from local storage. 
            ParticipantsCollectionView.Source = new ParticipantsCollectionViewSource(participants);
        }
    }
}