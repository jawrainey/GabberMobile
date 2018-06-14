using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Gabber.iOS.ViewSources
{
    public class ConsentViewSource : UITableViewSource
    {
        // When consent row has been selected notify the ViewController
        public Action<int> ConsentSelected;
        public List<Consent> Rows { get; set; }
        // Used to uncheck the row once a different one is selected
        NSIndexPath lastSelection;

        public ConsentViewSource(List<Consent> data) => Rows = data;

        public override nint NumberOfSections(UITableView tableView) => 1;

        public override nint RowsInSection(UITableView tableview, nint section) => Rows.Count;

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ConsentSelected(indexPath.Row);
            if (lastSelection != null) {
                tableView.CellAt(lastSelection).AccessoryView = new UIImageView(UIImage.FromBundle("CheckboxUnchecked"));
            }
            lastSelection = indexPath;
            tableView.CellAt(lastSelection).AccessoryView = new UIImageView(UIImage.FromBundle("CheckboxChecked"));
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(ConsentViewCell.Key, indexPath) as ConsentViewCell;
            cell.UpdateCell(Rows[indexPath.Row]);
            return cell;
        }
    }
}