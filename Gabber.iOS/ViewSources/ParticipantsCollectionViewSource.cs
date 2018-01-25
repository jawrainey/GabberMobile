using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL;

namespace Gabber.iOS.ViewSources
{
    public class ParticipantsCollectionViewSource : UICollectionViewSource
    {
        public List<Participant> Rows { get; set; }

        public ParticipantsCollectionViewSource(List<Participant> _rows)
        {
            Rows = _rows;
        }

        public override nint NumberOfSections(UICollectionView collectionView) 
        {
            // HACK: there does not seem to be a way to enable this within storyboard
            // and the CollectionView is not used directly. This is required such that
            // users can select more than one participant at a time...
            // This is here as this method will be invoked before the others below
            collectionView.AllowsMultipleSelection = true;
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Rows.Count;
        }

        public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ToggleSelected(collectionView, indexPath);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ToggleSelected(collectionView, indexPath);
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ParticipantsCollectionViewCell)collectionView.DequeueReusableCell(ParticipantsCollectionViewCell.CellID, indexPath);                
            cell.UpdateContent(Rows[indexPath.Row]);
            return cell;
        }

        // Helper method to simplify shared logic between
        void ToggleSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Rows[indexPath.Row].Selected = !Rows[indexPath.Row].Selected;
            Session.Connection.Update(Rows[indexPath.Row]);
            collectionView.ReloadData();
        }
    }
}
