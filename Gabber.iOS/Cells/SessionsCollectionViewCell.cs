using System;
using System.Collections.Generic;
using System.Globalization;
using Foundation;
using GabberPCL;
using GabberPCL.Models;
using UIKit;

namespace Gabber.iOS
{
    public partial class SessionsCollectionViewCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("SessionCollectionCell");

        public SessionsCollectionViewCell(IntPtr handle) : base(handle) { }

        string BuildParticipantsNames(List<InterviewParticipant> participants)
        {
            if (participants.Count == 1) return participants[0].Name.Split(' ')[0];
            var PartNames = new List<string>();
            foreach (var p in participants) PartNames.Add(Queries.UserById(p.UserID).Name.Split(' ')[0].Trim());
            return string.Join(", ", PartNames);
        }

        public void UpdateContent(InterviewSession session)
        {
            var content = LanguageChoiceManager.ContentByLanguage(Queries.ProjectById(session.ProjectID));
            SessionProjectTitle.Text = content.Title;
            SessionLength.Text = Queries.FormatFromSeconds(session.Prompts[session.Prompts.Count - 1].End);
            SessionParticipants.Text = BuildParticipantsNames(session.Participants);
            SessionCreateDate.Text = session.CreatedAt.ToString("MM/dd, HH:mm", CultureInfo.InvariantCulture);

            if (session.IsUploading)
            {
                SessionContainerView.BackgroundColor = UIColor.LightGray;
                SessionIsUploadedIndicator.StartAnimating();
            }
            else
            {
                SessionContainerView.BackgroundColor = UIColor.White;
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
