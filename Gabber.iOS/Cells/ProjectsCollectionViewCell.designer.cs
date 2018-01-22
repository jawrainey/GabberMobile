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
    [Register ("ProjectsCollectionViewCell")]
    partial class ProjectsCollectionViewCell
    {
        [Outlet]
        UIKit.UILabel ProjectDetails { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ProjectTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProjectDetails != null) {
                ProjectDetails.Dispose ();
                ProjectDetails = null;
            }

            if (ProjectTitle != null) {
                ProjectTitle.Dispose ();
                ProjectTitle = null;
            }
        }
    }
}