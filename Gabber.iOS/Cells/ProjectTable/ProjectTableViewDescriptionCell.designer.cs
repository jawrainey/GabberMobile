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
    [Register ("ProjectTableViewDescriptionCell")]
    partial class ProjectTableViewDescriptionCell
    {
        [Outlet]
        UIKit.UILabel ProjectDescription { get; set; }


        [Outlet]
        UIKit.UILabel TeaseLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProjectDescription != null) {
                ProjectDescription.Dispose ();
                ProjectDescription = null;
            }

            if (TeaseLabel != null) {
                TeaseLabel.Dispose ();
                TeaseLabel = null;
            }
        }
    }
}