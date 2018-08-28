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
    [Register ("ConsentViewCell")]
    partial class ConsentViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConsentViewCellSubtitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConsentViewCellTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ConsentViewCellSubtitle != null) {
                ConsentViewCellSubtitle.Dispose ();
                ConsentViewCellSubtitle = null;
            }

            if (ConsentViewCellTitle != null) {
                ConsentViewCellTitle.Dispose ();
                ConsentViewCellTitle = null;
            }
        }
    }
}