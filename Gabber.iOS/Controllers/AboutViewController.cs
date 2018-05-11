using System;
using Foundation;
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

            AboutContent.Text = StringResources.about_ui_content;
			AboutURLDescription.Text = StringResources.about_ui_url_description;
         
			AboutURL.DataDetectorTypes = UIDataDetectorType.Link;
		    AboutURL.Text = StringResources.about_ui_url;

            Title = StringResources.common_menu_about;
            TabBarController.Title = StringResources.about_ui_title;
		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            TabBarController.Title = StringResources.about_ui_title;
        }
    }
}