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
    [Register ("FirstDebriefViewController")]
    partial class FirstDebriefViewController
    {
        [Outlet]
        UIKit.UILabel CongratsBody { get; set; }


        [Outlet]
        UIKit.UILabel CongratsTitle { get; set; }


        [Outlet]
        UIKit.UILabel ConsentBody1 { get; set; }


        [Outlet]
        UIKit.UILabel ConsentBody2 { get; set; }


        [Outlet]
        UIKit.UILabel ConsentBody3 { get; set; }


        [Outlet]
        UIKit.UILabel ConsentTitle { get; set; }


        [Outlet]
        UIKit.UIButton FinishedButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CongratsBody != null) {
                CongratsBody.Dispose ();
                CongratsBody = null;
            }

            if (CongratsTitle != null) {
                CongratsTitle.Dispose ();
                CongratsTitle = null;
            }

            if (ConsentBody1 != null) {
                ConsentBody1.Dispose ();
                ConsentBody1 = null;
            }

            if (ConsentBody2 != null) {
                ConsentBody2.Dispose ();
                ConsentBody2 = null;
            }

            if (ConsentBody3 != null) {
                ConsentBody3.Dispose ();
                ConsentBody3 = null;
            }

            if (ConsentTitle != null) {
                ConsentTitle.Dispose ();
                ConsentTitle = null;
            }

            if (FinishedButton != null) {
                FinishedButton.Dispose ();
                FinishedButton = null;
            }
        }
    }
}