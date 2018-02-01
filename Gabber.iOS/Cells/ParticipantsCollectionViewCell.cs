using Foundation;
using System;
using UIKit;
using GabberPCL;
using GabberPCL.Models;

namespace Gabber.iOS
{
    public partial class ParticipantsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("ParticipantCollectionCell");

        public ParticipantsCollectionViewCell (IntPtr handle) : base (handle) {}

        public void UpdateContent(User participant)
        {
            ParticipantName.Text = participant.Name;
            BackgroundColor = participant.Selected ? UIColor.Green : UIColor.Red;
        }
    }
}