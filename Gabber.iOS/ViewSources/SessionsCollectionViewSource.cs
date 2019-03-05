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

        public void SessionIsUploading(int position) => Sessions[position].IsUploading = true;

        public void SessionUploadFail(int position)
        {
            Sessions[position].IsUploaded = false;
            Sessions[position].IsUploading = false;
        }

        public void SessionIsUploaded(int position)
        {
            Sessions[position].IsUploaded = true;
            Sessions[position].IsUploading = false;
            // Update state so the session isnt shown on reload etc.
            GabberPCL.Session.Connection.Update(Sessions[position]);
            Sessions.Remove(Sessions[position]);
        }

        public SessionsCollectionViewSource(List<InterviewSession> _sessions) => Sessions = _sessions;

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath) => SelectSession(indexPath.Row);

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => Sessions.Count;

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (SessionsCollectionViewCell)collectionView.DequeueReusableCell(SessionsCollectionViewCell.CellID, indexPath);
            cell.Layer.BorderWidth = 1.0f;
            cell.Layer.BorderColor = Application.MainColour;
            cell.UpdateContent(Sessions[indexPath.Row]);
            return cell;
        }
    }
}
