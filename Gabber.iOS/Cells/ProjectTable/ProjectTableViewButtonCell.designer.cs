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
