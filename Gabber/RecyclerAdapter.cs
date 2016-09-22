using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

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
			mholder.mprojectText.Text = _projects[position].theme;
		}

		public override int ItemCount
		{
			get { return _projects.Count; }
		}

		public event EventHandler<int> ProjectClicked;

		void OnProjectClick(int position)
		{
			if (ProjectClicked != null) ProjectClicked(this, position);
		}

		public class ProjectViewHolder : RecyclerView.ViewHolder
		{
			public ImageView mprojectCard { get; set; }
			public TextView mprojectText { get; set; }

			public ProjectViewHolder(View view, Action<int> listener) : base(view)
			{
				mprojectCard = view.FindViewById<ImageView>(Resource.Id.imagePrompt);
				mprojectText = view.FindViewById<TextView>(Resource.Id.imageText);
				// Binds event to each card within the UI
				mprojectCard.Click += (sender, e) => listener(LayoutPosition);
			}
		}
	}
}