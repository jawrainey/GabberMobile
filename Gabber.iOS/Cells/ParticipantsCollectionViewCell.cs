using Foundation;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class ParticipantsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("ParticipantCollectionCell");

        public ParticipantsCollectionViewCell (IntPtr handle) : base (handle) {}

        public void UpdateContent(string _participantObject)
        {
            ParticipantName.Text = _participantObject;
            // TODO: 
            // 1) Find participant from local database
            // 2) Update participant with new object details
        }
    }
}