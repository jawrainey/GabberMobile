// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
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
			if (CongratsTitle != null) {
				CongratsTitle.Dispose ();
				CongratsTitle = null;
			}

			if (CongratsBody != null) {
				CongratsBody.Dispose ();
				CongratsBody = null;
			}

			if (ConsentTitle != null) {
				ConsentTitle.Dispose ();
				ConsentTitle = null;
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

			if (FinishedButton != null) {
				FinishedButton.Dispose ();
				FinishedButton = null;
			}
		}
	}
}
