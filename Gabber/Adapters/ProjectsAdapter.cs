using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using GabberPCL.Models;
using Java.Lang;
using Android.Content;
using Android.Graphics;
using GabberPCL.Resources;
using FFImageLoading.Views;
using FFImageLoading;
using FFImageLoading.Transformations;

namespace Gabber.Adapters
{
    public class ProjectsAdapter : BaseExpandableListAdapter
    {
        private List<Project> projects;
        private long randomOffset;
        private Context context;

        public ProjectsAdapter(List<Project> projects, Context context)
        {
            this.context = context;
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
                viewHolder.PromptLayout = convertView.FindViewById<LinearLayout>(Resource.Id.project_topics);
                viewHolder.Button = convertView.FindViewById<Button>(Resource.Id.project_startBtn);

                //TODO non-english translations
                convertView.FindViewById<TextView>(Resource.Id.topics_tease).Text = StringResources.projects_ui_topics;

                convertView.Tag = viewHolder;
            }

            viewHolder.DescriptionText.Text = projects[groupPosition].Description;
            viewHolder.PromptLayout.RemoveAllViews();

            foreach (Prompt prompt in projects[groupPosition].Prompts)
            {
                TextView textView = new TextView(context);

                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);

                layoutParams.SetMargins(0, 1, 0, 1);
                textView.LayoutParameters = layoutParams;
                textView.SetMinHeight(100);
                textView.SetPadding(15, 15, 15, 15);
                textView.SetBackgroundColor(Color.WhiteSmoke); ;
                textView.Gravity = GravityFlags.Center;

                textView.Text += prompt.Text;

                viewHolder.PromptLayout.AddView(textView);

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

                viewHolder.Image = convertView.FindViewById<ImageViewAsync>(Resource.Id.project_icon);

                convertView.Tag = viewHolder;
            }

            Project thisProj = projects[groupPosition];

            viewHolder.Title.Text = thisProj.Title;

            ImageService.Instance.LoadCompiledResource("ic_launcher").Transform(new CircleTransformation()).Into(viewHolder.Image);

            if (!string.IsNullOrWhiteSpace(thisProj.image))
            {
                ImageService.Instance.LoadUrl(thisProj.image).Transform(new CircleTransformation()).Into(viewHolder.Image);
            }

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
        public ImageViewAsync Image { get; set; }
    }

    public class ProjectChildViewHolder : Java.Lang.Object
    {
        public TextView DescriptionText { get; set; }
        public LinearLayout PromptLayout { get; set; }
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

            //TODO non-english translations
            Button.Text = StringResources.projects_ui_get_started_button;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            buttonClicked?.Invoke(parentIndex);
        }
    }
}