using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL.Models;

namespace Gabber.iOS.ViewSources
{
    public class SessionsCollectionViewSource : UICollectionViewSource
    {
        public List<InterviewSession> Sessions { get; set; }

        public SessionsCollectionViewSource(List<InterviewSession> _sessions)
        {
             Sessions = _sessions;
        }

        public override nint NumberOfSections(UICollectionView _) => 1;

        public override nint GetItemsCount(UICollectionView __, nint _) => Sessions.Count;

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (SessionsCollectionViewCell)collectionView.DequeueReusableCell(SessionsCollectionViewCell.CellID, indexPath);
            cell.Layer.BorderWidth = 1.0f;
            cell.Layer.BorderColor = UIColor.Black.CGColor;
            cell.UpdateContent(Sessions[indexPath.Row]);
            return cell;
        }
    }
}
