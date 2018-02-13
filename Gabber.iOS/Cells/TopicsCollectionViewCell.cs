using Foundation;
using System;
using UIKit;
using GabberPCL.Models;

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
                BackgroundColor = UIColor.FromRGB(.43f, .80f, .79f);
                Layer.BorderColor = UIColor.LightGray.CGColor;
            }
            else if (topic.SelectionState == Prompt.SelectedState.previous) 
            {
                BackgroundColor = UIColor.LightGray;
            }
            else {
                BackgroundColor = UIColor.White;                
            }
        }
    }
}