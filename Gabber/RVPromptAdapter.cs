using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System;

namespace Gabber
{
	public class RVPromptAdapter : RecyclerView.Adapter
	{
		// Each story the user recorded has an associated image and audio.
		readonly List<Tuple<string, int>> _prompts;

		public RVPromptAdapter(List<Tuple<string, int>> prompts)
		{
			_prompts = prompts;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.prompt, parent, false);
			return new PhotoViewHolder(row);
		}
		
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var vh = holder as PhotoViewHolder;
			vh.Caption.Text = _prompts[position].Item1;
			vh.Image.SetImageResource(_prompts[position].Item2);
			// Required to lookup the drawable resource (image prompt) by ID.
			vh.Image.Tag = _prompts[position].Item2;
		}

		public override int ItemCount
		{
			get { return _prompts.Count; }
		}

		public class PhotoViewHolder : RecyclerView.ViewHolder
		{
			public ImageView Image { get; set; }
			public TextView Caption { get; set; }

			public PhotoViewHolder(View itemView) : base(itemView)
			{
				Image = itemView.FindViewById<ImageView>(Resource.Id.imagePrompt);
				Caption = itemView.FindViewById<TextView>(Resource.Id.caption);
			}
		}
	}
}