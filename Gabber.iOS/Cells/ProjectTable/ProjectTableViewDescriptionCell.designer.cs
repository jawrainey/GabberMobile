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
	[Register ("ProjectTableViewDescriptionCell")]
	partial class ProjectTableViewDescriptionCell
	{
		[Outlet]
		UIKit.UILabel ProjectDescription { get; set; }

		[Outlet]
		UIKit.UILabel TeaseLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProjectDescription != null) {
				ProjectDescription.Dispose ();
				ProjectDescription = null;
			}

			if (TeaseLabel != null) {
				TeaseLabel.Dispose ();
				TeaseLabel = null;
			}
		}
	}
}
