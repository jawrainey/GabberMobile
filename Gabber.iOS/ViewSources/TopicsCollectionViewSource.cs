using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL;

namespace Gabber.iOS.ViewSources
{
    public class TopicsCollectionViewSource : UICollectionViewSource
    {
        // TODO: this should be a Project from the PCL
        public List<Prompt> Rows { get; set; }

        public TopicsCollectionViewSource(List<Prompt> _rows)
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
            // TODO: once a TOPIC is clicked, save to local SelectedAnnotations
            // Not sure if a delegate is best suited for this?
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (TopicsCollectionViewCell)collectionView.DequeueReusableCell(TopicsCollectionViewCell.CellID, indexPath);
            cell.UpdateContent(Rows[indexPath.Row].prompt);
            return cell;
        }
    }
}
