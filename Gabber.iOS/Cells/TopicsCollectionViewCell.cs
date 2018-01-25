using Foundation;
using System;
using UIKit;
using GabberPCL;

namespace Gabber.iOS
{
    public partial class TopicsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("TopicCollectionCell");

        public TopicsCollectionViewCell (IntPtr handle) : base (handle) {}

        public void UpdateContent(Prompt topic)
        {
            ProjectTopic.Text = topic.prompt;

            if (topic.SelectionState == Prompt.SelectedState.current)
            {
                BackgroundColor = UIColor.Green;
            }
            else if (topic.SelectionState == Prompt.SelectedState.previous) 
            {
                BackgroundColor = UIColor.Yellow;
            }
            else {
                BackgroundColor = UIColor.Red;                
            }
        }
    }
}