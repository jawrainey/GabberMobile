using Foundation;
using System;
using UIKit;
using GabberPCL;

namespace Gabber.iOS
{
    public partial class ParticipantsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("ParticipantCollectionCell");

        public ParticipantsCollectionViewCell (IntPtr handle) : base (handle) {}

        public void UpdateContent(Participant participant)
        {
            ParticipantName.Text = participant.Name;
            BackgroundColor = participant.Selected ? UIColor.Green : UIColor.Red;
        }
    }
}