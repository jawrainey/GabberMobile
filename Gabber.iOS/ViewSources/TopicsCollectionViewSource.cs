using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL.Models;

namespace Gabber.iOS.ViewSources
{
    public class TopicsCollectionViewSource : UICollectionViewSource
    {
        public Action AddAnnotation;
        public List<Prompt> Rows { get; set; }

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
            int previousSelected = Rows.FindIndex((Prompt p) => p.SelectionState == Prompt.SelectedState.current);
            // Current & previous items
            var selectedItems = new List<NSIndexPath> { indexPath };
            // At least two items must be selected before a previous exists
            if (previousSelected != -1)
            {
                // The item selected was the same as the last (nothing changed) so do nothing.
                if (Rows[previousSelected].Equals(Rows[indexPath.Row])) return;
                Rows[previousSelected].SelectionState = Prompt.SelectedState.previous;
                selectedItems.Add(NSIndexPath.FromRowSection(previousSelected, 0));
            }
            Rows[indexPath.Row].SelectionState = Prompt.SelectedState.current;
            // Reloads (i.e. draws) the specific items, including those outside of the scrollview.
            collectionView.ReloadItems(selectedItems.ToArray());
            // Invoked after as this requires knowing if the Current/Previous was selected, particularly the first time.
            AddAnnotation?.Invoke();
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (TopicsCollectionViewCell)collectionView.DequeueReusableCell(TopicsCollectionViewCell.CellID, indexPath);
            cell.Layer.BorderWidth = 1.0f;
            cell.Layer.BorderColor = UIColor.Black.CGColor;
            cell.UpdateContent(Rows[indexPath.Row]);
            return cell;
        }
    }
}
