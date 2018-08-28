using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
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
            Project thisProj = Rows[indexPath.Section];

            if (indexPath.Row == 0)
            {
                // Description cell
                var descCell = (ProjectTableViewDescriptionCell)tableView.DequeueReusableCell(
                    ProjectTableViewDescriptionCell.CellID, indexPath);
                descCell.UpdateContent(thisProj.Description);
                return descCell;
            }

            int promptInd = indexPath.Row - 1;

            if (promptInd < thisProj.Prompts.Count)
            {
                // A prompt
                var promptCell = (ProjectTableViewPromptCell)tableView.DequeueReusableCell(
                    ProjectTableViewPromptCell.CellID, indexPath);
                promptCell.UpdateContent(thisProj.Prompts[promptInd].Text);
                return promptCell;
            }
            else
            {
                // last cell is the 'Get Started' button
                var buttonCell = (ProjectTableViewButtonCell)tableView.DequeueReusableCell(ProjectTableViewButtonCell.CellID, indexPath);
                buttonCell.UpdateContent(thisProj.ID, StartProject);
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
            Project project = Rows[(int)section];

            return (project.IsExpanded) ? project.Prompts.Count + 2 : 0;
        }
    }
}
