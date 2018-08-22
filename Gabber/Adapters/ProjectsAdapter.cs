using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using GabberPCL.Models;
using Java.Lang;

namespace Gabber.Adapters
{
    public class ProjectsAdapter : BaseExpandableListAdapter
    {
        private List<Project> projects;
        private long randomOffset;

        public ProjectsAdapter(List<Project> projects)
        {
            this.randomOffset = new Random().Next(100000, 1000000);
            this.projects = projects;
        }

        public void UpdateProjects(List<Project> projects)
        {
            this.projects = projects;
            NotifyDataSetChanged();
        }

        public override int GroupCount => projects.Count;

        public override bool HasStableIds => true;

        public override int ChildTypeCount => 1;

        public event EventHandler<int> ProjectClicked;

        private void OnProjectClick(int position)
        {
            Console.WriteLine("BUTTON CLICK AT POS " + position);
            ProjectClicked?.Invoke(this, position);
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return projects[groupPosition].ID + randomOffset;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return 1;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            int viewType = GetChildType(groupPosition, childPosition);
            ProjectChildViewHolder viewHolder = (ProjectChildViewHolder)convertView?.Tag;

            if (viewHolder == null)
            {
                viewHolder = new ProjectChildViewHolder();

                convertView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.project_descriptionCell, parent, false);
                viewHolder.DescriptionText = convertView.FindViewById<TextView>(Resource.Id.project_description);
                viewHolder.PromptText = convertView.FindViewById<TextView>(Resource.Id.project_topics);
                viewHolder.Button = convertView.FindViewById<Button>(Resource.Id.project_startBtn);

                convertView.Tag = viewHolder;
            }

            viewHolder.DescriptionText.Text = projects[groupPosition].Description;

            viewHolder.PromptText.Text = "";

            foreach (Prompt prompt in projects[groupPosition].Prompts)
            {
                viewHolder.PromptText.Text += "\n" + prompt.Text + "\n";
            }

            viewHolder.SetUpButton(OnProjectClick, groupPosition);

            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return null;
        }

        public override long GetGroupId(int groupPosition)
        {
            return projects[groupPosition].ID;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            ProjectParentViewHolder viewHolder = (ProjectParentViewHolder)convertView?.Tag;

            if (viewHolder == null)
            {
                viewHolder = new ProjectParentViewHolder();

                convertView = LayoutInflater.From(parent.Context)
                                            .Inflate(Resource.Layout.project_headercell, parent, false);
                viewHolder.Title = convertView.FindViewById<TextView>(Resource.Id.project_title);
                // TODO image

                convertView.Tag = viewHolder;
            }

            Project thisProj = projects[groupPosition];

            viewHolder.Title.Text = thisProj.Title;

            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return GetChildType(groupPosition, childPosition) == 2;
        }

    }

    public class ProjectParentViewHolder : Java.Lang.Object
    {
        public TextView Title { get; set; }
        public ImageView Image { get; set; }
    }

    public class ProjectChildViewHolder : Java.Lang.Object
    {
        public TextView DescriptionText { get; set; }
        public TextView PromptText { get; set; }
        public Button Button { get; set; }
        private int parentIndex;
        private Action<int> buttonClicked;

        public void SetUpButton(Action<int> clicked, int parentInd)
        {
            if (Button == null) return;

            buttonClicked = clicked;
            parentIndex = parentInd;
            Button.Click -= Button_Click;
            Button.Click += Button_Click;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            buttonClicked?.Invoke(parentIndex);
        }
    }
}