using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using GabberPCL.Models;

namespace Gabber.Adapters
{
	public class ProjectsAdapter : RecyclerView.Adapter
	{
		List<Project> _projects;

        public ProjectsAdapter(List<Project> projects)
		{
			_projects = projects;
		}

        public void UpdateProjects(List<Project> projects)
        {
            _projects = projects;
            NotifyDataSetChanged();
        }

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
            var project = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.prompt, parent, false);
			return new ProjectViewHolder(project, OnProjectClick);
		}
		
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var mholder = holder as ProjectViewHolder;
            mholder.ProjectTitle.Text = _projects[position].Title;
		}

        public override int ItemCount => (_projects != null ? _projects.Count : 0);

		public event EventHandler<int> ProjectClicked;

        void OnProjectClick(int position) => ProjectClicked?.Invoke(this, position);

		public class ProjectViewHolder : RecyclerView.ViewHolder
		{
            public TextView ProjectTitle { get; set; }

			public ProjectViewHolder(View view, Action<int> listener) : base(view)
			{
                view.Click += (sender, e) => listener(LayoutPosition);
                ProjectTitle = view.FindViewById<TextView>(Resource.Id.caption);
			}
		}
	}
}