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
        UIKit.UILabel AboutTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AboutContent != null) {
                AboutContent.Dispose ();
                AboutContent = null;
            }

            if (AboutTitle != null) {
                AboutTitle.Dispose ();
                AboutTitle = null;
            }
        }
    }
}