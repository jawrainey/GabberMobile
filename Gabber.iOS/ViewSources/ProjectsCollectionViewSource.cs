using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL.Models;
using Gabber.iOS.Helpers;

namespace Gabber.iOS.ViewSources
{
    public class ProjectsCollectionViewSource : UICollectionViewSource
    {
        public List<Project> Rows { get; set; }

        public ProjectsCollectionViewSource(List<Project> _rows)
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
            Logger.LOG_EVENT_WITH_ACTION("PROJECT_SELECTED", Rows[indexPath.Row].Title, "PROJECT");
            NSUserDefaults.StandardUserDefaults.SetInt(Rows[indexPath.Row].ID, "SelectedProjectID");
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ProjectsCollectionViewCell)collectionView.DequeueReusableCell(ProjectsCollectionViewCell.CellID, indexPath);
            cell.Layer.BorderWidth = 1.0f;
            cell.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            cell.UpdateContent(Rows[indexPath.Row].Title);
            return cell;
        }
    }
}
