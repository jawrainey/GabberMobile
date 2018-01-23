using System;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace Gabber.iOS.ViewSources
{
    public class ParticipantsCollectionViewSource : UICollectionViewSource
    {
        // TODO: this should be a Participant object from PCL
        public List<string> Rows { get; set; }

        public ParticipantsCollectionViewSource(List<string> _rows)
        {
            Rows = _rows;
        }

        public override nint NumberOfSections(UICollectionView collectionView) 
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Rows.Count;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Console.WriteLine("I have been selected & i am a participant");
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ParticipantsCollectionViewCell)collectionView.DequeueReusableCell(ParticipantsCollectionViewCell.CellID, indexPath);
            cell.UpdateContent(Rows[indexPath.Row]);
            return cell;
        }
    }
}
