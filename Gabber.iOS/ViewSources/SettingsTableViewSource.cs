using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Foundation;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using UIKit;
using Gabber.iOS.Helpers;
using System.Threading;

namespace Gabber.iOS.ViewSources
{
    public class SettingsTableViewSource : UITableViewSource
    {
        readonly List<SettingsCell> Options;
        readonly UIViewController Context;
        readonly Action<int> WasClicked;

        public SettingsTableViewSource(List<SettingsCell> options, UIViewController context, Action<int> callBack)
        {
            Options = options;
            Context = context;
            WasClicked = callBack;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => Options.Count;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SettingsCell");
            var isLogoutOrAboutCells = (indexPath.Row == Options.Count - 1) || indexPath.Row == 0;

            var type = isLogoutOrAboutCells ? UITableViewCellStyle.Default : UITableViewCellStyle.Subtitle;
            cell = new UITableViewCell(type, "SettingsCell") { SelectionStyle = UITableViewCellSelectionStyle.None };

            cell.TextLabel.Text = Options[indexPath.Row].Title;

            if (!isLogoutOrAboutCells)
            {
                cell.DetailTextLabel.Text = Options[indexPath.Row].Subtitle;
                cell.DetailTextLabel.TextColor = UIColor.FromCGColor(Application.MainColour);
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath) => WasClicked?.Invoke(indexPath.Row);
    }
}
