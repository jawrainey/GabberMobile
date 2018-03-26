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
        List<InterviewSession> Sessions;

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

            var project = Queries.ProjectById(Sessions[position].ProjectID);
            var session = Sessions[position];
            var project_title = project.Title;
            var num_topics = session.Prompts.Count;
            var num_parts = session.Participants.Count;
            var length = session.Prompts[num_topics - 1].End;

            var PartNames = new List<string>();
            foreach (var p in session.Participants) PartNames.Add(Queries.UserById(p.UserID).Name);

            mholder.UploadProgress.Visibility = ViewStates.Gone;
            mholder.SessionUploaded.Visibility = ViewStates.Visible;

            if (session.IsUploading && !(session.IsUploaded))
            {
                mholder.UploadProgress.Visibility = ViewStates.Visible;
                mholder.SessionUploaded.Visibility = ViewStates.Gone;
            }

            mholder.Participants.Text = "(" + num_parts.ToString() + ") " + string.Join(", ", PartNames);
            mholder.Length.Text = TimeSpan.FromSeconds(length).ToString((@"mm\:ss"));
            mholder.ProjectTitle.Text = project_title;
            mholder.NumTopics.Text = num_topics.ToString() + " Topics";
            mholder.SessionUploaded.SetBackgroundResource(session.IsUploaded ? Resource.Drawable.cloud_done : Resource.Drawable.cloud_upload);
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
            NotifyItemChanged(position);
        }

        public override int ItemCount => (Sessions != null ? Sessions.Count : 0);

        public class SessionViewHolder : RecyclerView.ViewHolder
        {
            public TextView Participants { get; set; }
            public TextView Length { get; set; }
            public TextView ProjectTitle { get; set; }
            public TextView NumTopics { get; set; }
            public AppCompatButton SessionUploaded { get; set; }
            public ProgressBar UploadProgress { get; set; }

            public SessionViewHolder(View item) : base(item)
            {
                Participants = item.FindViewById<TextView>(Resource.Id.session_participants);
                Length = item.FindViewById<TextView>(Resource.Id.session_length);
                ProjectTitle = item.FindViewById<TextView>(Resource.Id.project_title);
                NumTopics = item.FindViewById<TextView>(Resource.Id.session_num_topics);
                SessionUploaded = item.FindViewById<AppCompatButton>(Resource.Id.session_uploaded);
                UploadProgress = item.FindViewById<ProgressBar>(Resource.Id.upload_progress);
            }
        }
    }
}
