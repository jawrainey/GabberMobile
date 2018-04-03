using System;
using UIKit;

namespace Gabber.iOS
{
	public partial class SessionsViewController : UICollectionViewController
	{
		public SessionsViewController (IntPtr handle) : base (handle) {}

		public override void ViewDidAppear(bool animated)
		{
            base.ViewDidAppear(animated);
            TabBarController.Title = "Your Gabbers";
		}
	}
}