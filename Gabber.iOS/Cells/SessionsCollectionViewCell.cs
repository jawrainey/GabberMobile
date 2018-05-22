using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        string BuildParticipantsNames(List<InterviewParticipant> participants)
        {
            if (participants.Count == 1) return participants[0].Name.Split(' ')[0];
            var PartNames = new List<string>();
            foreach (var p in participants) PartNames.Add(Queries.UserById(p.UserID).Name.Split(' ')[0].Trim());
            return string.Join(", ", PartNames);
        }

        public void UpdateContent(InterviewSession session)
        {
            SessionProjectTitle.Text = Queries.ProjectById(session.ProjectID).Title;
            SessionLength.Text = Queries.FormatFromSeconds(session.Prompts[session.Prompts.Count - 1].End);
            SessionParticipants.Text = BuildParticipantsNames(session.Participants);
            SessionCreateDate.Text = session.CreatedAt.ToString("MM/dd, HH:mm");

            if (session.IsUploading) SessionIsUploadedIndicator.StartAnimating();
            else SessionIsUploadedIndicator.StopAnimating();
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
