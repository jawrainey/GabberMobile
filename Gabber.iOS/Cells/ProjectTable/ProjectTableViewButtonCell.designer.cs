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
    [Register ("ProjectTableViewButtonCell")]
    partial class ProjectTableViewButtonCell
    {
        [Outlet]
        UIKit.UIButton GetStartedButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GetStartedButton != null) {
                GetStartedButton.Dispose ();
                GetStartedButton = null;
            }
        }
    }
}