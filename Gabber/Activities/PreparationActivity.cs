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

namespace Gabber
{
	[Activity]
	public class PreparationActivity : AppCompatActivity
	{
        // Expose for on-click event to update participants view
        ParticipantAdapter adapter;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			SupportActionBar.Title = Resources.GetText(Resource.String.hint_who_gabbering_with);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			// Required to access existing gabbers for a given user
			var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var participants = Queries.AllParticipantsUnSelected();
			var participantsView = FindViewById<RecyclerView>(Resource.Id.participants);
            participantsView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            adapter = new ParticipantAdapter(participants);
			adapter.ParticipantClicked += ParticipantSelected;
			participantsView.SetAdapter(adapter);
            UpdateParticipantsSelectedLabel();
            new LinearSnapHelper().AttachToRecyclerView(participantsView);

			FindViewById<Button>(Resource.Id.addParticipant).Click += delegate
			{
                var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Add a new participant");
                alert.SetView(Resource.Layout.participantdialog);
                // Set this to null now to enable override of click button later
                alert.SetPositiveButton("Add", (EventHandler<DialogClickEventArgs>)null);

                alert.SetNegativeButton("Cancel", (dialog, id) => 
                {
                    ((Android.Support.V7.App.AlertDialog)dialog).Dismiss(); 
                });

                var AddParticipantDialog = alert.Create();
                AddParticipantDialog.Show();
                // Override the on click such that we can dismiss the dialog from here, otherwise
                // it dismisses every time the button is clicked and we cannot do validation.
                AddParticipantDialog.GetButton((int)DialogButtonType.Positive).Click += delegate
                {
                    var name = AddParticipantDialog.FindViewById<TextInputEditText>(Resource.Id.participantName);
                    var email = AddParticipantDialog.FindViewById<TextInputEditText>(Resource.Id.participantEmail);

                    if (FormValid(name, email))
                    {
                        var participant = new User
                        {
                            Name = name.Text,
                            Email = email.Text,
                            Selected = true
                        };

                        Session.Connection.Insert(participant);
                        participants.Add(participant);
                        adapter.NotifyItemInserted(participants.Count);

                        // Reset form content once participant is successfully added
                        name.Text = "";
                        email.Text = "";
                        AddParticipantDialog.Dismiss();
                    }
                };
			};

			FindViewById<Button>(Resource.Id.selectPrompt).Click += delegate
			{
                if (adapter.SelectedParticipantsCount == 0)
				{
                    Toast.MakeText(this, Resources.GetText(Resource.String.select_participant), ToastLength.Long).Show();
				}
                else if (adapter.SelectedParticipantsCount == 1)
                {
                    var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alert.SetTitle("One participant selected");
                    alert.SetMessage("Are you sure that you only want to select one participant?");
                    alert.SetIcon(Android.Resource.Drawable.IcDialogAlert);

                    alert.SetPositiveButton("Continue", (dialog, id) =>
                    {
                        StartActivity(new Intent(this, typeof(RecordStoryActivity)));
                    });

                    alert.SetNegativeButton("Cancel", (dialog, id) =>
                    {
                        ((Android.Support.V7.App.AlertDialog)dialog).Dismiss();
                    });

                    alert.Create().Show();
                }
				else
				{
                    StartActivity(new Intent(this, typeof(RecordStoryActivity)));
				}
			};
		}

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            OnBackPressed();
            return true;
        }

        bool FormValid(TextInputEditText name, TextInputEditText email)
        {
            if (string.IsNullOrWhiteSpace(name.Text))
            {
                name.Error = Resources.GetText(Resource.String.error_empty_name);
                return false;
            }

            if (string.IsNullOrWhiteSpace(email.Text))
            {
                email.Error = Resources.GetText(Resource.String.error_empty_email);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(email.Text) && !Patterns.EmailAddress.Matcher(email.Text).Matches())
            {
                email.Error = Resources.GetText(Resource.String.error_invalid_email);
                return false;
            }
            return true;
        }

        void UpdateParticipantsSelectedLabel()
        {
            var partCount = FindViewById<TextView>(Resource.Id.participantCount);
            partCount.Text = string.Format("{0} participants selected", adapter.SelectedParticipantsCount);
        }

        void ParticipantSelected(object sender, int position)
        {
            adapter.ParticipantSeleted(position);
            UpdateParticipantsSelectedLabel();
        }
    }
}