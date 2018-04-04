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
    [Register ("ProjectsViewController")]
    partial class ProjectsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView ProjectsActivityIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView ProjectsCollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ProjectsInstructions { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProjectsActivityIndicator != null) {
                ProjectsActivityIndicator.Dispose ();
                ProjectsActivityIndicator = null;
            }

            if (ProjectsCollectionView != null) {
                ProjectsCollectionView.Dispose ();
                ProjectsCollectionView = null;
            }

            if (ProjectsInstructions != null) {
                ProjectsInstructions.Dispose ();
                ProjectsInstructions = null;
            }
        }
    }
}