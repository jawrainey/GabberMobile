using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GabberPCL;
using GabberPCL.Models;

namespace Gabber.Adapters
{
    public class SessionAdapter : RecyclerView.Adapter
    {
        public List<InterviewSession> Sessions;

        public SessionAdapter(List<InterviewSession> _sessions)
        {
            Sessions = _sessions;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var session = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.sessions_rv, parent, false);
            return new SessionViewHolder(session);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var mholder = holder as SessionViewHolder;
            var session = Sessions[position];

            mholder.UploadProgress.Visibility = session.IsUploading ? ViewStates.Visible : ViewStates.Gone;
            mholder.Participants.Text = BuildParticipantsNames(session.Participants);
            mholder.DateCreated.Text = session.CreatedAt.ToString("MM/dd, HH:mm");
            mholder.Length.Text = TimeSpan.FromSeconds(session.Prompts[session.Prompts.Count - 1].End).ToString(@"mm\:ss");
            mholder.ProjectTitle.Text = Queries.ProjectById(Sessions[position].ProjectID).Title;
        }

        string BuildParticipantsNames(List<InterviewParticipant> participants)
        {
            if (participants.Count == 1) return participants[0].Name.Split(' ')[0];
            var PartNames = new List<string>();
            foreach (var p in participants) PartNames.Add(Queries.UserById(p.UserID).Name.Split(' ')[0].Trim());

            return string.Join(", ", PartNames);
        }

        public void SessionIsUploading(int position)
        {
            Sessions[position].IsUploading = true;
            NotifyItemChanged(position);
        }

        public void SessionUploadFail(int position)
        {
            Sessions[position].IsUploaded = false;
            Sessions[position].IsUploading = false;
            NotifyItemChanged(position);
        }

        public void SessionIsUploaded(int position)
        {
            Sessions[position].IsUploaded = true;
            Sessions[position].IsUploading = false;
            Session.Connection.Update(Sessions[position]);
            Sessions.Remove(Sessions[position]);
            NotifyDataSetChanged();
        }

        public override int ItemCount => (Sessions != null ? Sessions.Count : 0);

        public class SessionViewHolder : RecyclerView.ViewHolder
        {
            public TextView Participants { get; set; }
            public TextView Length { get; set; }
            public TextView ProjectTitle { get; set; }
            public TextView DateCreated { get; set; }
            public ProgressBar UploadProgress { get; set; }

            public SessionViewHolder(View item) : base(item)
            {
                Participants = item.FindViewById<TextView>(Resource.Id.session_participants);
                Length = item.FindViewById<TextView>(Resource.Id.session_length);
                ProjectTitle = item.FindViewById<TextView>(Resource.Id.project_title);
                DateCreated = item.FindViewById<TextView>(Resource.Id.session_date_created);
                UploadProgress = item.FindViewById<ProgressBar>(Resource.Id.upload_progress);
            }
        }
    }
}
