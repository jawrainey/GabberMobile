using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL.Models;

namespace Gabber.iOS.ViewSources
{
    public class SessionsCollectionViewSource : UICollectionViewSource
    {
        public Action<int> SelectSession;
        public List<InterviewSession> Sessions { get; set; }

        public SessionsCollectionViewSource(List<InterviewSession> _sessions)
        {
             Sessions = _sessions;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            SelectSession(indexPath.Row);
        }

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => Sessions.Count;

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (SessionsCollectionViewCell)collectionView.DequeueReusableCell(SessionsCollectionViewCell.CellID, indexPath);
            cell.Layer.BorderWidth = 1.0f;
            cell.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            cell.UpdateContent(Sessions[indexPath.Row]);
            return cell;
        }
    }
}
