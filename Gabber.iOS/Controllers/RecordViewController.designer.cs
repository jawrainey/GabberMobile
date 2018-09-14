// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Gabber.iOS
{
	[Register ("RecordViewController")]
	partial class RecordViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel InterviewTimer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton RecordButton { get; set; }

		[Outlet]
		UIKit.UILabel ThemeTitleLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UICollectionView TopicsCollectionView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel TopicsInstructions { get; set; }

		[Action ("RecordingCompleteDialog:")]
		partial void RecordingCompleteDialog (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (InterviewTimer != null) {
				InterviewTimer.Dispose ();
				InterviewTimer = null;
			}

			if (RecordButton != null) {
				RecordButton.Dispose ();
				RecordButton = null;
			}

			if (TopicsCollectionView != null) {
				TopicsCollectionView.Dispose ();
				TopicsCollectionView = null;
			}

			if (TopicsInstructions != null) {
				TopicsInstructions.Dispose ();
				TopicsInstructions = null;
			}

			if (ThemeTitleLabel != null) {
				ThemeTitleLabel.Dispose ();
				ThemeTitleLabel = null;
			}
		}
	}
}
