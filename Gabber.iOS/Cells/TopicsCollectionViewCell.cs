using Foundation;
using System;
using UIKit;
using GabberPCL.Models;
using System.Drawing;

namespace Gabber.iOS
{
    public partial class TopicsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("TopicCollectionCell");

        public TopicsCollectionViewCell (IntPtr handle) : base (handle) {}

        public void UpdateContent(Prompt topic)
        {
            ProjectTopic.Text = topic.Text;
            ProjectTopic.TextColor = UIColor.Black;
            Layer.BorderColor = UIColor.Black.CGColor;

            if (topic.SelectionState == Prompt.SelectedState.current)
            {
                ProjectTopic.TextColor = UIColor.White;
                ProjectTopic.BackgroundColor = UIColor.FromRGB(.43f, .80f, .79f);
                Layer.BorderColor = UIColor.LightGray.CGColor;
            }
            else if (topic.SelectionState == Prompt.SelectedState.previous) 
            {
                ProjectTopic.BackgroundColor = UIColor.LightGray;
            }
            else 
            {
                ProjectTopic.BackgroundColor = UIColor.White;
            }
        }

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