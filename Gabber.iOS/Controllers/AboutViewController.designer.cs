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
    [Register ("AboutViewController")]
    partial class AboutViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AboutContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView AboutURL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AboutURLDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView SettingsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AboutContent != null) {
                AboutContent.Dispose ();
                AboutContent = null;
            }

            if (AboutURL != null) {
                AboutURL.Dispose ();
                AboutURL = null;
            }

            if (AboutURLDescription != null) {
                AboutURLDescription.Dispose ();
                AboutURLDescription = null;
            }

            if (SettingsTableView != null) {
                SettingsTableView.Dispose ();
                SettingsTableView = null;
            }
        }
    }
}