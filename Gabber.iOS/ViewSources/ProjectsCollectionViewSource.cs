using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL.Models;

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
            NSUserDefaults.StandardUserDefaults.SetInt(Rows[indexPath.Row].ID, "SelectedProjectID");
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ProjectsCollectionViewCell)collectionView.DequeueReusableCell(ProjectsCollectionViewCell.CellID, indexPath);
            cell.UpdateContent(Rows[indexPath.Row].Title);
            return cell;
        }
    }
}
