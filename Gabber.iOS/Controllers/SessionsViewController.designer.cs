// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Gabber.iOS
{
    [Register ("SessionsViewController")]
    partial class SessionsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView SessionsCollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SessionsInstructions { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SessionsInstructionsBody { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SessionsUpload { get; set; }

        [Action ("UploadAll:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UploadAll (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (SessionsCollectionView != null) {
                SessionsCollectionView.Dispose ();
                SessionsCollectionView = null;
            }

            if (SessionsInstructions != null) {
                SessionsInstructions.Dispose ();
                SessionsInstructions = null;
            }

            if (SessionsInstructionsBody != null) {
                SessionsInstructionsBody.Dispose ();
                SessionsInstructionsBody = null;
            }

            if (SessionsUpload != null) {
                SessionsUpload.Dispose ();
                SessionsUpload = null;
            }
        }
    }
}