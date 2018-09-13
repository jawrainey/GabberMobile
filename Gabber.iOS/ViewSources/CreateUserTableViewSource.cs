using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Gabber.iOS.ViewSources
{
    public class CreateUserTableViewSource : UITableViewSource
    {
        public class UserOption
        {
            public string Title;
            public string ShownData;
            public Action OnClick;
        }

        public List<UserOption> Rows;

        public CreateUserTableViewSource(List<UserOption> data)
        {
            Rows = data;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UserOption thisOption = Rows[indexPath.Row];

            var optionCell = (RegisterOptionCell)tableView.DequeueReusableCell(
                RegisterOptionCell.CellID, indexPath);
            optionCell.UpdateContent(thisOption);

            return optionCell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Rows.Count;
        }
    }
}
