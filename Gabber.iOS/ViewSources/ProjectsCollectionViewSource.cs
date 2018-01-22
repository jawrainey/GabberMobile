using System;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace Gabber.iOS.ViewSources
{
    public class ProjectsCollectionViewSource : UICollectionViewSource
    {
        // TODO: if this were an object it would be different
        public List<string> Rows { get; set; }

        public ProjectsCollectionViewSource(List<string> _rows)
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

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            System.Console.WriteLine(ProjectsCollectionViewCell.CellID);
            var cell = (ProjectsCollectionViewCell)collectionView.DequeueReusableCell(ProjectsCollectionViewCell.CellID, indexPath);
            cell.UpdateContent(Rows[indexPath.Row]);
            return cell;
        }

    }
}
