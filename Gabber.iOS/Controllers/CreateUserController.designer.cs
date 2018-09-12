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
	[Register ("CreateUserController")]
	partial class CreateUserController
	{
		[Outlet]
		UIKit.UIButton FinishButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FinishButton != null) {
				FinishButton.Dispose ();
				FinishButton = null;
			}
		}
	}
}
