using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using FFImageLoading;
using FFImageLoading.Views;
using System;
using Android.Graphics;

namespace Gabber
{
	public class RVPromptAdapter : RecyclerView.Adapter
	{
		// Each story the user recorded has an associated image and audio.
		readonly List<GabberPCL.Prompt> _prompts;
        int lastSelectedPosition = int.MinValue;

		public RVPromptAdapter(List<GabberPCL.Prompt> prompts)
		{
			_prompts = prompts;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.prompt, parent, false);
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

            if (position == lastSelectedPosition)
            {
                // CURRENT SELECTED STATE [item that was just selected]
                vh.Caption.SetBackgroundColor(Color.ParseColor("#26A69A"));
                vh.Caption.SetTextColor(Color.White);
            }
            else if (_prompts[position].Selected)
            {
                // PREVIOUS SELECTED STATE [item was selected before]
                vh.Caption.SetBackgroundColor(Color.LightGray);
                vh.Caption.SetTextColor(Color.Black);
            }
            else {
                // DEFAULT STATE [the item has never been selected]
                vh.Caption.SetBackgroundResource(Resource.Drawable.promptBorder);
                vh.Caption.SetTextColor(Color.Black);
            }
		}

        public void PromptSeleted(int position)
        {
            _prompts[position].Selected = true;
            int temp = lastSelectedPosition;
            lastSelectedPosition = position;
            if(temp != int.MinValue)
                NotifyItemChanged(temp);
            NotifyItemChanged(position);
        }

		public override int ItemCount
		{
			get { return _prompts.Count; }
		}

		public event EventHandler<int> ProjectClicked;

        void OnProjectClick(int position) => ProjectClicked?.Invoke(this, position);

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