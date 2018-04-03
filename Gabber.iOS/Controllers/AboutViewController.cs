using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class AboutViewController : UIViewController
    {
        public AboutViewController (IntPtr handle) : base (handle){}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            TabBarController.Title = "About Gabber";
        }
    }
}