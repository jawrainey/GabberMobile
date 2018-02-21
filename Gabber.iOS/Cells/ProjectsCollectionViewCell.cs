using Foundation;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class ProjectsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("ProjectCollectionCell");

        public ProjectsCollectionViewCell (IntPtr handle) : base (handle) {}

        public void UpdateContent(string _title) => ProjectDetails.Text = _title;

        public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(UICollectionViewLayoutAttributes layoutAttributes)
        {
            var autoLayoutAttributes = base.PreferredLayoutAttributesFittingAttributes(layoutAttributes);
            var targetSize = new CoreGraphics.CGSize(layoutAttributes.Frame.Width, 0);
            var autoLayoutSize = ContentView.SystemLayoutSizeFittingSize(targetSize, 1000, 250);
            var autoLayoutFrame = new CoreGraphics.CGRect(autoLayoutAttributes.Frame.Location, autoLayoutSize);
            autoLayoutAttributes.Frame = autoLayoutFrame;
            return autoLayoutAttributes;
        }
    }
}