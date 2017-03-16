using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using GabberPCL;
using Android.Widget;
using Refractored.Controls;
using Android.Media;
using Android.Graphics;

namespace Gabber
{
	public class ParticipantAdapter : RecyclerView.Adapter
	{
		List<Participant> _participants;

		public ParticipantAdapter(List<Participant> participants)
		{
			_participants = participants;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var participant = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.participant, parent, false);
			return new ParticipantViewHolder(participant, OnParticipantClicked);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var mholder = holder as ParticipantViewHolder;
			mholder.Name.Text = _participants[position].Name;

			var photoPath = _participants[position].Photo;
			if (!string.IsNullOrEmpty(photoPath))
			{
				mholder.Photo.SetImageBitmap(ThumbnailUtils.ExtractThumbnail(
					BitmapFactory.DecodeFile(photoPath, new BitmapFactory.Options { InSampleSize = 8 }), 74, 74));
			}
		}

		public override int ItemCount
		{
			get { return (_participants != null ? _participants.Count : 0); }
		}

		public event EventHandler<int> ParticipantClicked;

		void OnParticipantClicked(int position)
		{
			if (ParticipantClicked != null) ParticipantClicked(this, position);
		}

		public class ParticipantViewHolder : RecyclerView.ViewHolder
		{
			public TextView Name { get; set; }
			public CircleImageView Photo { get; set; }

			public ParticipantViewHolder(View item, Action<int> listener) : base(item)
			{
				item.Click += (sender, e) => listener(LayoutPosition);
				Name = item.FindViewById<TextView>(Resource.Id.name);
				Photo = item.FindViewById<CircleImageView>(Resource.Id.photo);
			}
		}
	}
}
