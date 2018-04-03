using System;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    public partial class AboutViewController : UIViewController
    {
        public AboutViewController (IntPtr handle) : base (handle){}

		public override void ViewDidLoad()
		{
            base.ViewDidLoad();

            AboutTitle.Text = StringResources.about_ui_title;
            AboutContent.Text = StringResources.about_ui_content;
		}

		public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            TabBarController.Title = "About Gabber";
        }
    }
}