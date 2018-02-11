using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using GabberPCL.Models;

namespace Gabber
{
	public class RecyclerAdapter : RecyclerView.Adapter
	{
		List<Project> _projects;

		public RecyclerAdapter(List<Project> projects)
		{
			_projects = projects;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View project = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.project, parent, false);
			return new ProjectViewHolder(project, OnProjectClick);
		}
		
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var mholder = holder as ProjectViewHolder;
            mholder.mprojectText.Text = _projects[position].Title;
		}

        public override int ItemCount => _projects.Count;

		public event EventHandler<int> ProjectClicked;

        void OnProjectClick(int position) => ProjectClicked?.Invoke(this, position);

		public class ProjectViewHolder : RecyclerView.ViewHolder
		{
			public TextView mprojectText { get; set; }

			public ProjectViewHolder(View view, Action<int> listener) : base(view)
			{
				mprojectText = view.FindViewById<TextView>(Resource.Id.imageText);
				// Binds event to each card within the UI
				mprojectText.Click += (sender, e) => listener(LayoutPosition);
			}
		}
	}
}