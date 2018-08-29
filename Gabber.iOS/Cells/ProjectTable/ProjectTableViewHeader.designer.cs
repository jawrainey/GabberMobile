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
    [Register ("ProjectTableViewHeader")]
    partial class ProjectTableViewHeader
    {
        [Outlet]
        UIKit.UILabel ArrowLabel { get; set; }


        [Outlet]
        UIKit.UIImageView ProjectIcon { get; set; }


        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ArrowLabel != null) {
                ArrowLabel.Dispose ();
                ArrowLabel = null;
            }

            if (ProjectIcon != null) {
                ProjectIcon.Dispose ();
                ProjectIcon = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }
        }
    }
}