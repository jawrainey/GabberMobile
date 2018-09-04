using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS.ViewSources
{
    public class SettingsTableViewSource : UITableViewSource
    {
        string[] options = {
            StringResources.settings_about,
            StringResources.settings_chooseAppLanguage,
            StringResources.settings_chooseConvoLanguage,
            StringResources.settings_logout
        };

        readonly UIViewController Context;
        readonly Action<NSUrl> OpenWebPage;

        public SettingsTableViewSource(UIViewController Controller, Action<NSUrl> CallBack)
        {
            OpenWebPage = CallBack;
            Context = Controller;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => options.Length;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SettingsCell");

            if (cell == null)
            {
                var isLogoutOrAboutCells = (indexPath.Row == options.Length - 1) || indexPath.Row == 0;

                //
                var type = isLogoutOrAboutCells ? UITableViewCellStyle.Default : UITableViewCellStyle.Subtitle;
                cell = new UITableViewCell(type, "SettingsCell")
                {
                    SelectionStyle = UITableViewCellSelectionStyle.None
                };

                cell.TextLabel.Text = options[indexPath.Row];

                if (!isLogoutOrAboutCells)
                {
                    cell.DetailTextLabel.Text = "English";
                    cell.DetailTextLabel.TextColor = UIColor.FromRGB(.43f, .80f, .79f);
                }
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            switch (indexPath.Row)
            {
                case 0:
                    OpenWebPage?.Invoke(new NSUrl("https://gabber.audio/about"));
                    break;
                case 1:
                    Console.WriteLine("Change application language");
                    break;
                case 2:
                    Console.WriteLine("Change default conversation language");
                    break;
                case 3:
                    Console.WriteLine("Log out");
                    break;
            }
        }
    }
}
