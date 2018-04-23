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

namespace Gabber
{
	[Activity]
    public class PreparationActivity : AppCompatActivity
	{
        // Expose for on-click event to update participants view
        ParticipantAdapter adapter;
        // Expose for on-click event to update participants view
        List<User> participants;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.preparation);
            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = StringResources.participants_ui_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var partsInstructs = FindViewById<TextView>(Resource.Id.participantsInstructions);
            partsInstructs.Text = StringResources.participants_ui_instructions;

			// Required to access existing gabbers for a given user
			var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            participants = Queries.AllParticipantsUnSelected();
			var participantsView = FindViewById<RecyclerView>(Resource.Id.participants);
            participantsView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            adapter = new ParticipantAdapter(participants);
			adapter.ParticipantClicked += ParticipantSelected;
			participantsView.SetAdapter(adapter);
            UpdateParticipantsSelectedLabel();
            new LinearSnapHelper().AttachToRecyclerView(participantsView);

            var startRecording = FindViewById<Button>(Resource.Id.startRecording);
            startRecording.Text = StringResources.participants_ui_startrecording_button;

            startRecording.Click += delegate
			{
                if (adapter.SelectedParticipantsCount == 0)
				{
                    Toast.MakeText(this, StringResources.participants_ui_validation_noneselected, ToastLength.Long).Show();
				}
                else if (adapter.SelectedParticipantsCount == 1)
                {
                    var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alert.SetTitle(StringResources.participants_ui_validation_oneselected_title);
                    alert.SetMessage(StringResources.participants_ui_validation_oneselected_message);
                    alert.SetIcon(Android.Resource.Drawable.IcDialogAlert);

                    alert.SetPositiveButton(StringResources.participants_ui_validation_oneselected_continue, (dialog, id) =>
                    {
                        StartActivity(new Intent(this, typeof(RecordStoryActivity)));
                    });

                    alert.SetNegativeButton(StringResources.participants_ui_validation_oneselected_cancel, (dialog, id) =>
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

        void ShowAddParticipantDialog()
        {
            var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle(StringResources.participants_ui_dialog_add_title);
            var _dialog = LayoutInflater.Inflate(Resource.Layout.participantdialog, null);

            var _fullname = _dialog.FindViewById<TextInputLayout>(Resource.Id.participantNameLayout);
            _fullname.Hint = StringResources.register_ui_fullname_label;
            var _email = _dialog.FindViewById<TextInputLayout>(Resource.Id.participantEmailLayout);
            _email.Hint = StringResources.common_ui_forms_email_label;

            alert.SetView(_dialog);
            // Set this to null now to enable override of click button later
            alert.SetPositiveButton(StringResources.participants_ui_dialog_add_positive, (EventHandler<DialogClickEventArgs>)null);

            alert.SetNegativeButton(StringResources.participants_ui_dialog_add_negative, (dialog, id) =>
            {
                ((Android.Support.V7.App.AlertDialog)dialog).Dismiss();
            });

            var AddParticipantDialog = alert.Create();
            AddParticipantDialog.Window.SetSoftInputMode(SoftInput.StateAlwaysVisible);
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
                        Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.Text),
                        Email = email.Text,
                        Selected = true
                    };

                    Session.Connection.Insert(participant);
                    participants.Add(participant);
                    adapter.NotifyItemInserted(participants.Count);

                    // Reset form content once participant is successfully added
                    name.Text = "";
                    email.Text = "";
                    UpdateParticipantsSelectedLabel();
                    AddParticipantDialog.Dismiss();
                }
            };
        }

        bool FormValid(TextInputEditText name, TextInputEditText email)
        {
            if (string.IsNullOrWhiteSpace(name.Text))
            {
                name.Error = StringResources.register_ui_fullname_validate_empty;
                return false;
            }

            if (string.IsNullOrWhiteSpace(email.Text))
            {
                email.Error = StringResources.common_ui_forms_email_validate_empty;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(email.Text) && !Patterns.EmailAddress.Matcher(email.Text).Matches())
            {
                email.Error = StringResources.common_ui_forms_email_validate_invalid;
                return false;
            }
            return true;
        }

        void UpdateParticipantsSelectedLabel()
        {
            var partCount = FindViewById<TextView>(Resource.Id.participantCount);
            partCount.Text = string.Format(StringResources.participants_ui_numselected, adapter.SelectedParticipantsCount);
        }

        void ParticipantSelected(object sender, int position)
        {
            adapter.ParticipantSeleted(position);
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
                    ShowAddParticipantDialog();
                    return true;
                default:
                    OnBackPressed();
                    return true;
            }
        }
    }
}