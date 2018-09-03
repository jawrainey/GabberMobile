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
    [Register ("ProjectTableViewController")]
    partial class ProjectTableViewController
    {
        [Outlet]
        UIKit.UILabel blurbLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (blurbLabel != null) {
                blurbLabel.Dispose ();
                blurbLabel = null;
            }
        }
    }
}