using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System;
using Android.Graphics;
using GabberPCL.Models;

namespace Gabber.Adapters
{
	public class TopicAdapter : RecyclerView.Adapter
	{
		// Each story the user recorded has an associated image and audio.
        readonly List<Topic> _prompts;
        int lastSelectedPosition = int.MinValue;

        public TopicAdapter(List<Topic> prompts)
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
            vh.Caption.Text = _prompts[position].Text;

            if (position == lastSelectedPosition)
            {
                _prompts[position].SelectionState = Topic.SelectedState.current;
                // CURRENT SELECTED STATE [item that was just selected]
                vh.Caption.SetBackgroundColor(Color.ParseColor("#26A69A"));
                vh.Caption.SetTextColor(Color.White);
            }
            else if (_prompts[position].Selected)
            {
                _prompts[position].SelectionState = Topic.SelectedState.previous;
                // PREVIOUS SELECTED STATE [item was selected before]
                vh.Caption.SetBackgroundColor(Color.LightGray);
                vh.Caption.SetTextColor(Color.Black);
            }
            else {
                _prompts[position].SelectionState = Topic.SelectedState.never;
                // DEFAULT STATE [the item has never been selected]
                vh.Caption.SetBackgroundResource(Resource.Drawable.record_topic_border);
                vh.Caption.SetTextColor(Color.Black);
            }
		}

        public void PromptSeleted(int position)
        {
            _prompts[position].Selected = true;
            _prompts[position].SelectionState = Topic.SelectedState.current;
            int previous = lastSelectedPosition;
            lastSelectedPosition = position;
            if(previous != int.MinValue)
                NotifyItemChanged(previous);
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
			public TextView Caption { get; set; }

			public PhotoViewHolder(View itemView, Action<int> listener) : base(itemView)
			{
				itemView.Click += (sender, e) => listener(LayoutPosition);
				Caption = itemView.FindViewById<TextView>(Resource.Id.caption);
			}
		}
	}
}