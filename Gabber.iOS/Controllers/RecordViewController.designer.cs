// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

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
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView TopicsCollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TopicsInstructions { get; set; }

        [Action ("RecordingCompleteDialog:")]
        [GeneratedCode ("iOS Designer", "1.0")]
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
        }
    }
}