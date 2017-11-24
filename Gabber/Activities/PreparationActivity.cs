using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Widget;
using System.Collections.Generic;
using GabberPCL;
using Android.Preferences;
using Android.Support.V7.Widget;
using Android.Views;
using Newtonsoft.Json;
using Android.Util;

namespace Gabber
{
	[Activity]
	public class PreparationActivity : AppCompatActivity
	{
		List<Participant> _participants;
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

            // This is pre-populated with the current user once they register and therefore SHOULD exist
            _participants = JsonConvert.DeserializeObject<List<Participant>>(prefs.GetString("participants", ""));

            // Hide the existing participant title
            if (_participants.Count <= 1) {
                FindViewById<TextView>(Resource.Id.selectExistingParticipant).Visibility = ViewStates.Gone;
                FindViewById<LinearLayout>(Resource.Id.ParticipantContentWidget).Visibility = ViewStates.Visible;
                FindViewById<AppCompatButton>(Resource.Id.selectPrompt).Enabled = false;
            }

			var participantsView = FindViewById<RecyclerView>(Resource.Id.participants);
			participantsView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

			adapter = new ParticipantAdapter(_participants);
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
					var participant = new Participant
					{
						Name = name.Text,
						Email = email.Text,
                        Selected = true
					};

                    // It is the first time a participant was added so we will enable the button
                    if (_participants.Count <= 1)
                    {
                        FindViewById<AppCompatButton>(Resource.Id.selectPrompt).Enabled = true;
                    }

                    // Hide the form everytime a participant is added.
                    FindViewById<LinearLayout>(Resource.Id.ParticipantContentWidget).Visibility = ViewStates.Gone;
					_participants.Add(participant);
					adapter.NotifyDataSetChanged();

					prefs.Edit().PutString("participants", JsonConvert.SerializeObject(_participants)).Commit();
					// Reset form content once participant is successfully added
					name.Text = "";
					email.Text = "";
				}
			};

            FindViewById<TextView>(Resource.Id.createNewParticipant).Click += delegate
            {
                var widget = FindViewById<LinearLayout>(Resource.Id.ParticipantContentWidget);
                if (widget.Visibility == ViewStates.Gone) {
                    widget.Visibility = ViewStates.Visible;
                    FindViewById<TextView>(Resource.Id.createNewParticipant).Text = "Add a new participant (Hide)";
                } 
                else {

					widget.Visibility = ViewStates.Gone;
                    FindViewById<TextView>(Resource.Id.createNewParticipant).Text = "Add a new participant (Show)";
                }

            };

			FindViewById<Button>(Resource.Id.selectPrompt).Click += delegate
			{
                var selectedParticipants = adapter.selectedParticipants();

                if (selectedParticipants.Count == 0)
				{
					Snackbar.Make(name, Resources.GetText(Resource.String.select_participant), Snackbar.LengthLong).Show();
				}
				else
				{
					// Store created participants as these are displayed to the user on the UI.
					prefs.Edit().PutString("participants", JsonConvert.SerializeObject(_participants)).Commit();
					// Pass the preparation form and previously form data (theme) to the record activity.
					var intent = new Intent(this, typeof(PromptSelectionActivity));
                    // We do not want to store "you" as set in the register page, but instead the full name
                    selectedParticipants[0].Name = prefs.GetString("username", selectedParticipants[0].Name);
					intent.PutExtra("participants", JsonConvert.SerializeObject(selectedParticipants));
					intent.PutExtra("theme", prefs.GetString("theme", ""));
					intent.PutExtra("session", System.Guid.NewGuid().ToString());
					StartActivity(intent);
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
			if (!string.IsNullOrWhiteSpace(email.Text) && !Patterns.EmailAddress.Matcher(email.Text).Matches())
			{
				Snackbar.Make(email, Resources.GetText(Resource.String.error_invalid_email), Snackbar.LengthLong).Show();
				return false;
			}
			return true;
		}

		void ParticipantSelected(object sender, int position)
		{
            adapter.ParticipantSeleted(position);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}
