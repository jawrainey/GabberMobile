using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Refractored.Controls;
using Android.Graphics;
using GabberPCL.Models;

namespace Gabber
{
	public class ParticipantAdapter : RecyclerView.Adapter
	{
        List<User> _participants;

        public ParticipantAdapter(List<User> participants)
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

            if (_participants[position].Selected)
            {
                mholder.Name.SetBackgroundColor(Color.ParseColor("#26A69A"));
                mholder.Name.SetTextColor(Color.White);
            }
            else
            {
                mholder.Name.SetBackgroundResource(Resource.Drawable.record_topic_border);
                mholder.Name.SetTextColor(Color.Black);
            }
		}

        public void ParticipantSeleted(int index) 
        {
            _participants[index].Selected = !_participants[index].Selected;
            GabberPCL.Session.Connection.Update(_participants[index]);
            NotifyItemChanged(index);
        }

        public User GetByIndex(int index) {
            return (_participants != null && _participants.Count >= index ? _participants[index] : null);
        }

        public override int ItemCount => (_participants != null ? _participants.Count : 0);

		public event EventHandler<int> ParticipantClicked;

        void OnParticipantClicked(int position) => ParticipantClicked?.Invoke(this, position);

		public class ParticipantViewHolder : RecyclerView.ViewHolder
		{
			public TextView Name { get; set; }

			public ParticipantViewHolder(View item, Action<int> listener) : base(item)
			{
				item.Click += (sender, e) => listener(LayoutPosition);
				Name = item.FindViewById<TextView>(Resource.Id.name);
			}
		}
	}
}
