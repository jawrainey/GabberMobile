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
			if (ProjectIcon != null) {
				ProjectIcon.Dispose ();
				ProjectIcon = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (ArrowLabel != null) {
				ArrowLabel.Dispose ();
				ArrowLabel = null;
			}
		}
	}
}
