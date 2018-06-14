using Foundation;
using System;
using UIKit;

namespace Gabber.iOS
{
    public partial class ConsentViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("ConversationConsentCell");

        public ConsentViewCell (IntPtr handle) : base (handle) {}

        public void UpdateCell(Consent item)
        {
            AccessoryView = new UIImageView(UIImage.FromBundle("CheckboxUnchecked"));
            ConsentViewCellTitle.Text = item.Title;
            ConsentViewCellSubtitle.AttributedText = item.Subtitle;
        }
    }
}