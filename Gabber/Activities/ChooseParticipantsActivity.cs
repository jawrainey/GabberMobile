using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Widget;
using GabberPCL.Models;
using Android.Preferences;
using Android.Support.V7.Widget;
using GabberPCL;
using Android.Util;
using System;
using Android.Views;
using GabberPCL.Resources;
using System.Collections.Generic;
using System.Globalization;
using Firebase.Analytics;
using Android.Content.PM;
using Gabber.Activities;
using Android.Runtime;

namespace Gabber
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ChooseParticipantsActivity : AppCompatActivity
    {
        FirebaseAnalytics firebaseAnalytics;
        // Expose for on-click event to update participants view
        ParticipantAdapter adapter;
        // Expose for on-click event to update participants view
        List<User> participants;
        RecyclerView participantsView;

        private int AddParticipantReqCode = 101;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            firebaseAnalytics = FirebaseAnalytics.GetInstance(ApplicationContext);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.preparation);
            SupportActionBar.Title = StringResources.participants_ui_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var partsInstructs = FindViewById<TextView>(Resource.Id.participantsInstructions);
            partsInstructs.Text = StringResources.participants_ui_instructions;

            // Required to access existing gabbers for a given user
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

            participantsView = FindViewById<RecyclerView>(Resource.Id.participants);
            participantsView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            new LinearSnapHelper().AttachToRecyclerView(participantsView);

            var startRecording = FindViewById<Button>(Resource.Id.startRecording);
            startRecording.Text = StringResources.participants_ui_startrecording_button;

            startRecording.Click += StartRecording_Click;
            UpdateList();
        }

        private void UpdateList()
        {
            participants = Queries.AllParticipantsUnSelected();
            adapter = new ParticipantAdapter(participants);
            adapter.ParticipantClicked += ParticipantSelected;
            participantsView.SetAdapter(adapter);
            UpdateParticipantsSelectedLabel();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == AddParticipantReqCode && resultCode == Result.Ok)
            {
                string newEmail = data.GetStringExtra("NEW_EMAIL");
                User thisUser = Queries.UserByEmail(newEmail);
                participants.Add(thisUser);
                adapter.NotifyItemInserted(participants.Count);
                LOG_ADD_PARTICIPANT(thisUser.Name, thisUser.Email);
            }
        }

        void UpdateParticipantsSelectedLabel()
        {
            var partCount = FindViewById<TextView>(Resource.Id.participantCount);
            partCount.Text = string.Format(StringResources.participants_ui_numselected, adapter.SelectedParticipantsCount);
        }

        void ParticipantSelected(object sender, int position)
        {
            adapter.ParticipantSeleted(position);
            LOG_PARTICIPANT(position);
            UpdateParticipantsSelectedLabel();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.add_person, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.add_person:
                    StartActivityForResult(new Intent(this, typeof(AddParticipantActivity)), AddParticipantReqCode);
                    //ShowAddParticipantDialog();
                    return true;
                default:
                    OnBackPressed();
                    return true;
            }
        }

        void LOG_EVENT_WITH_ACTION(string eventName, string action)
        {
            var bundle = new Bundle();
            bundle.PutString("ACTION", action);
            firebaseAnalytics.LogEvent(eventName, bundle);
        }

        void LOG_ADD_PARTICIPANT(string name, string email)
        {
            var bundle = new Bundle();
            bundle.PutString("NAME", name);
            bundle.PutString("EMAIL", email);
            firebaseAnalytics.LogEvent("ADD_PARTICIPANT", bundle);
        }

        void LOG_PARTICIPANT(int position)
        {
            var bundle = new Bundle();
            bundle.PutString("NAME", participants[position].Name);
            bundle.PutString("EMAIL", participants[position].Email);
            bundle.PutBoolean("STATE", participants[position].Selected);
            bundle.PutInt("NUM_PARTICIPANTS", participants.Count);
            firebaseAnalytics.LogEvent("PARTICIPANT_SELECTED", bundle);
        }

        private void StartRecording_Click(object sender, EventArgs e)
        {
            if (adapter.SelectedParticipantsCount == 0)
            {
                LOG_EVENT_WITH_ACTION("NO_PARTICIPANTS_SELECTED", "TOAST");
                Toast.MakeText(this, StringResources.participants_ui_validation_noneselected, ToastLength.Long).Show();
            }
            else if (adapter.SelectedParticipantsCount == 1)
            {
                LOG_EVENT_WITH_ACTION("ONE_PARTICIPANT_MODAL", "DISPLAYED");
                var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle(StringResources.participants_ui_validation_oneselected_title);
                alert.SetMessage(StringResources.participants_ui_validation_oneselected_message);
                alert.SetIcon(Android.Resource.Drawable.IcDialogAlert);

                alert.SetPositiveButton(StringResources.participants_ui_validation_oneselected_continue, (dialog, id) =>
                {
                    LOG_EVENT_WITH_ACTION("ONE_PARTICIPANT_MODAL", "CONTINUE");
                    StartActivity(new Intent(this, typeof(Activities.ResearchConsent)));
                });

                alert.SetNegativeButton(StringResources.participants_ui_validation_oneselected_cancel, (dialog, id) =>
                {
                    LOG_EVENT_WITH_ACTION("ONE_PARTICIPANT_MODAL", "DISMISSED");
                    ((Android.Support.V7.App.AlertDialog)dialog).Dismiss();
                });

                alert.Create().Show();
            }
            else
            {
                LOG_EVENT_WITH_ACTION("NAVIGATE_TO_RECORD", "NAVIGATE");
                StartActivity(new Intent(this, typeof(Activities.ResearchConsent)));
            }
        }

    }
}