using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using FFImageLoading;
using FFImageLoading.Views;
using System;

namespace Gabber
{
	public class RVPromptAdapter : RecyclerView.Adapter
	{
		// Each story the user recorded has an associated image and audio.
		readonly List<GabberPCL.Prompt> _prompts;

		public RVPromptAdapter(List<GabberPCL.Prompt> prompts)
		{
			_prompts = prompts;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.prompt, parent, false);
			// Modify prompt size (60% of screen); required as the view is inflated inside another layout, 
			// so it is not possible (?) to modify the size of the prompt layout otherwise.
			var parms = row.LayoutParameters;
			parms.Width = (int)(parent.Width * 0.6);
			row.LayoutParameters = parms;
			return new PhotoViewHolder(row, OnProjectClick);
		}
		
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var vh = holder as PhotoViewHolder;
			vh.Caption.Text = _prompts[position].prompt;
			// Load the image from the web into the prompt imageView.
			ImageService.Instance.LoadUrl(_prompts[position].imageName).Into(vh.Image);
			// Required to lookup the drawable resource (image prompt) by ID.
			vh.Image.Tag = _prompts[position].imageName;
		}

		public override int ItemCount
		{
			get { return _prompts.Count; }
		}

		public event EventHandler<int> ProjectClicked;

		void OnProjectClick(int position)
		{
			if (ProjectClicked != null) ProjectClicked(this, position);
		}

		public class PhotoViewHolder : RecyclerView.ViewHolder
		{
			public ImageViewAsync Image { get; set; }
			public TextView Caption { get; set; }

			public PhotoViewHolder(View itemView, Action<int> listener) : base(itemView)
			{
				itemView.Click += (sender, e) => listener(LayoutPosition);
				Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.imagePrompt);
				Caption = itemView.FindViewById<TextView>(Resource.Id.caption);
			}
		}
	}
}