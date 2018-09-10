using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL;
using GabberPCL.Models;
using UIKit;

namespace Gabber.iOS.ViewSources
{
    public class ProjectsTableViewSource : UITableViewSource
    {
        public List<Project> Rows;
        private Action<Project> projectBtnTapped;
        private Action<int> handleCollapse;

        public ProjectsTableViewSource(List<Project> data, Action<Project> launchProj, Action<int> collapse) : base()
        {
            Rows = data;
            projectBtnTapped = launchProj;
            handleCollapse = collapse;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var project = Rows[indexPath.Section];

            var content = Queries.ContentByLanguage(project, Localize.GetCurrentCultureInfo());

            if (indexPath.Row == 0)
            {
                // Description cell
                var descCell = (ProjectTableViewDescriptionCell)tableView.DequeueReusableCell(
                    ProjectTableViewDescriptionCell.CellID, indexPath);
                descCell.UpdateContent(content.Description);
                return descCell;
            }

            int promptInd = indexPath.Row - 1;
            var activeTopics = content.Topics.Where((t) => t.IsActive).ToList();
            if (promptInd < activeTopics.Count)
            {
                // A prompt
                var promptCell = (ProjectTableViewPromptCell)tableView.DequeueReusableCell(
                    ProjectTableViewPromptCell.CellID, indexPath);
                promptCell.UpdateContent(activeTopics[promptInd].Text);
                return promptCell;
            }
            else
            {
                // last cell is the 'Get Started' button
                var buttonCell = (ProjectTableViewButtonCell)tableView.DequeueReusableCell(ProjectTableViewButtonCell.CellID, indexPath);
                buttonCell.UpdateContent(project.ID, StartProject);
                return buttonCell;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            try
            {
                var cell = (ProjectTableViewHeader)tableView.DequeueReusableCell(ProjectTableViewHeader.CellID);
                cell.UpdateContent(Rows[(int)section], HeaderTapped, section);
                cell.LayoutIfNeeded();

                return cell;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return UITableView.AutomaticDimension;
        }

        private void HeaderTapped(nint section)
        {
            Project thisProj = Rows[(int)section];
            thisProj.IsExpanded = !thisProj.IsExpanded;
            handleCollapse?.Invoke((int)section);
        }

        private void StartProject(int id)
        {
            projectBtnTapped?.Invoke(Rows.Find((obj) => obj.ID == id));
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return Rows.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            var project = Rows[(int)section];
            var content = Queries.ContentByLanguage(project, Localize.GetCurrentCultureInfo());
            var activeTopics = content.Topics.Where((t) => t.IsActive).ToList();
            return (project.IsExpanded) ? activeTopics.Count + 2 : 0;
        }
    }
}
