using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace Gabber.iOS
{
    public partial class ProjectsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("ProjectCollectionCell");

        public ProjectsCollectionViewCell (IntPtr handle) : base (handle) {}

        public void UpdateContent(string _title)
        {
            ProjectDetails.Text = _title;
        }
    }
}