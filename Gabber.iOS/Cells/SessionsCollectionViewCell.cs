using System;
using System.Collections.Generic;
using Foundation;
using GabberPCL;
using GabberPCL.Models;
using UIKit;

namespace Gabber.iOS
{
	public partial class SessionsCollectionViewCell : UICollectionViewCell
	{
        public static NSString CellID = new NSString("SessionCollectionCell");

		public SessionsCollectionViewCell (IntPtr handle) : base (handle){}

        public void UpdateContent(InterviewSession session)
        {
            var project = Queries.ProjectById(session.ProjectID);

            var PartNames = new List<string>();
            foreach (var p in session.Participants) PartNames.Add(Queries.UserById(p.UserID).Name);

            SessionParticipants.Text = $"({session.Participants.Count.ToString()}) {string.Join(", ", PartNames)}";
            SessionCreateDate.Text = session.CreatedAt.ToString("MM/dd, HH:mm");
            SessionLength.Text = TimeSpan.FromSeconds(session.Prompts.Count - 1).ToString((@"mm\:ss"));
            SessionProjectTitle.Text = project.Title;
            SessionNumTopics.Text = $"{session.Prompts.Count} Topics";


            if (session.IsUploading)
            {
                SessionIsUploaded.Hidden = true;
                SessionIsUploadedIndicator.StartAnimating();
            }
            else
            {
                SessionIsUploaded.Hidden = false;
                SessionIsUploadedIndicator.StopAnimating();
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
