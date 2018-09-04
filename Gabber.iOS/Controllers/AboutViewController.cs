using System;
using GabberPCL.Resources;
using SafariServices;
using UIKit;
using Gabber.iOS.ViewSources;

namespace Gabber.iOS
{
    public partial class AboutViewController : UIViewController
    {
        public AboutViewController (IntPtr handle) : base (handle){}

		public override void ViewDidLoad()
		{
            base.ViewDidLoad();

            Title = StringResources.common_menu_settings;
            TabBarController.Title = StringResources.about_ui_title;

            SettingsTableView.Source = new SettingsTableViewSource(
                this, (url) => PresentViewControllerAsync(new SFSafariViewController(url), true));
            SettingsTableView.SizeToFit();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            TabBarController.Title = StringResources.about_ui_title;
        }
    }
}