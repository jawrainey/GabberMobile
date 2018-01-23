using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL;

namespace Gabber.iOS.ViewSources
{
    public class ProjectsCollectionViewSource : UICollectionViewSource
    {
        // TODO: this should be a Project from the PCL
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
            NSUserDefaults.StandardUserDefaults.SetString(Rows[indexPath.Row].theme, "selectedProject");
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ProjectsCollectionViewCell)collectionView.DequeueReusableCell(ProjectsCollectionViewCell.CellID, indexPath);
            cell.UpdateContent(Rows[indexPath.Row].theme);
            return cell;
        }
    }
}
