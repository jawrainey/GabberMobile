using Foundation;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class TopicsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("TopicCollectionCell");

        public TopicsCollectionViewCell (IntPtr handle) : base (handle) {}

        public void UpdateContent(string topic)
        {
            ProjectTopic.Text = topic;
        }
    }
}