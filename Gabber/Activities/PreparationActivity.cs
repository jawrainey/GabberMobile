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
using Android.Util;
using GabberPCL;
using System.Collections.Generic;

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

			// Required to access existing gabbers for a given user
			var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var participants = Queries.AllParticipants();

            if (participants.Count <= 0)
            {
                Session.Connection.Insert(new User
                {
                    Name = "(You)",
                    Email = prefs.GetString("username", ""),
                    Selected = true
                });
                participants = Queries.AllParticipants();
            }

			var participantsView = FindViewById<RecyclerView>(Resource.Id.participants);
			participantsView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

            adapter = new ParticipantAdapter(participants);
			adapter.ParticipantClicked += ParticipantSelected;
			participantsView.SetAdapter(adapter);
			participantsView.AddItemDecoration(new DividerItemDecoration(participantsView.Context, 1));
			new LinearSnapHelper().AttachToRecyclerView(participantsView);

			var name = FindViewById<TextInputEditText>(Resource.Id.participantName);
			var email = FindViewById<TextInputEditText>(Resource.Id.participantEmail);

			FindViewById<Button>(Resource.Id.addParticipant).Click += delegate
			{
				if (FormValid())
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
				}
			};

			FindViewById<Button>(Resource.Id.selectPrompt).Click += delegate
			{
                var selectedParticipants = Queries.SelectedParticipants();

                if (selectedParticipants.Count == 0)
				{
					Snackbar.Make(name, Resources.GetText(Resource.String.select_participant), Snackbar.LengthLong).Show();
				}
				else
				{
                    StartActivity(new Intent(this, typeof(RecordStoryActivity)));
				}
			};
		}

		bool FormValid()
		{
			var name = FindViewById<TextInputEditText>(Resource.Id.participantName);
			var email = FindViewById<TextInputEditText>(Resource.Id.participantEmail);

			if (string.IsNullOrWhiteSpace(name.Text))
			{
				Snackbar.Make(name, Resources.GetText(Resource.String.error_friends_name), Snackbar.LengthLong).Show();
				return false;
			}

            if (string.IsNullOrWhiteSpace(email.Text))
            {
                Snackbar.Make(name, "Their email address is required and will be used to obtain their consent for the recording.", Snackbar.LengthLong).Show();
                return false;                
            }

			if (!string.IsNullOrWhiteSpace(email.Text) && !Patterns.EmailAddress.Matcher(email.Text).Matches())
			{
				Snackbar.Make(email, Resources.GetText(Resource.String.error_invalid_email), Snackbar.LengthLong).Show();
				return false;
			}
			return true;
		}

        void ParticipantSelected(object sender, int position) => adapter.ParticipantSeleted(position);
    }
}
