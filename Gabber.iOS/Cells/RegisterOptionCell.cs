// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using static Gabber.iOS.ViewSources.CreateUserTableViewSource;

namespace Gabber.iOS
{
    public partial class RegisterOptionCell : UITableViewCell
    {
        public static NSString CellID = new NSString("RegisterOptionCell");

        public RegisterOptionCell(IntPtr handle) : base(handle)
        {
        }

        public void UpdateContent(UserOption data)
        {
            TitleLabel.Text = data.Title;
            ContentLabel.Text = data.ShownData;
        }
    }
}
