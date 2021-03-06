// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using GabberPCL.Resources;

namespace Gabber.iOS
{
    public partial class ProjectTableViewDescriptionCell : UITableViewCell
    {
        public static NSString CellID = new NSString("ProjectTableViewDescriptionCell");

        public ProjectTableViewDescriptionCell(IntPtr handle) : base(handle) {}

        public void UpdateContent(string desc)
        {
            TeaseLabel.Text = StringResources.projects_ui_topics;
            ProjectDescription.Text = desc;
        }
    }
}
