using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL.Models;
using Gabber.iOS.Helpers;

namespace Gabber.iOS.ViewSources
{
    public class TopicsCollectionViewSource : UICollectionViewSource
    {
        public Action AddAnnotation;
        public List<Topic> Rows { get; set; }

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
            var previousSelected = Rows.FindIndex((p) => p.SelectionState == Topic.SelectedState.current);
            // At least two items must be selected before a previous exists
            if (previousSelected != -1)
            {
                // The item selected was the same as the last (nothing changed) so do nothing.
                if (Rows[previousSelected].Equals(Rows[indexPath.Row])) return;
                Rows[previousSelected].SelectionState = Topic.SelectedState.previous;
            }
            Rows[indexPath.Row].SelectionState = Topic.SelectedState.current;
            // Reloads (i.e. draws) the specific items, including those outside of the scrollview.
            collectionView.ReloadData();

            LOG_TOPIC_SELECTED(Rows[indexPath.Row]);

            // Invoked after as this requires knowing if the Current/Previous was selected, particularly the first time.
            AddAnnotation?.Invoke();
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (TopicsCollectionViewCell)collectionView.DequeueReusableCell(TopicsCollectionViewCell.CellID, indexPath);
            cell.Layer.BorderWidth = 1.0f;
            cell.Layer.BorderColor = Application.MainColour;
            cell.UpdateContent(Rows[indexPath.Row]);
            return cell;
        }

        void LOG_TOPIC_SELECTED(Topic current)
        {
            NSString[] keys = {
                new NSString("TEXT"),
                new NSString("ID"),
                new NSString("TIMESTAMP")
            };

            NSObject[] values = {
                new NSString(current.Text),
                new NSString(current.ID.ToString()),
                new NSString(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys, keys.Length);
            Logger.LOG_EVENT_WITH_DICT("TOPIC_SELECTED", parameters);
        }
    }
}