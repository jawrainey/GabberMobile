using System;
using UIKit;
using Gabber.iOS.ViewSources;
using GabberPCL;
using Foundation;
using GabberPCL.Resources;
using Gabber.iOS.Helpers;

namespace Gabber.iOS
{
    public partial class ParticipantsViewController : UIViewController
    {
        bool ConfirmOneParticipant;

        ParticipantsCollectionViewSource participantsViewSource;

        public ParticipantsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var es = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 36, 70);
            (ParticipantsCollectionView.CollectionViewLayout as UICollectionViewFlowLayout).EstimatedItemSize = es;

            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, a) =>
                {
                    PerformSegue("CreateParticipantSegue", this);
                }), true);

            RecordGabberButton.Layer.BorderWidth = 1.0f;
            RecordGabberButton.Layer.BorderColor = Application.MainColour;

            Title = StringResources.participants_ui_title;

            ParticipantsInstructions.Text = StringResources.participants_ui_instructions;
            RecordGabberButton.SetTitle(StringResources.participants_ui_startrecording_button, UIControlState.Normal);
            participantsViewSource = new ParticipantsCollectionViewSource(Queries.AllParticipantsUnSelected());
            participantsViewSource.AddParticipant += (int num) =>
            {

                NumSelectedParts.Text = string.Format(StringResources.participants_ui_numselected, num);
            };
            ParticipantsCollectionView.Source = participantsViewSource;
            UpdateNumSelectedPartsLabel();
        }

        // i.e. they navigated to here or they segued from adding parts
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            participantsViewSource.Rows = Queries.AllParticipants();
            UpdateNumSelectedPartsLabel();
            ParticipantsCollectionView.ReloadData();
        }

        void UpdateNumSelectedPartsLabel()
        {
            NumSelectedParts.Text = string.Format(StringResources.participants_ui_numselected, Queries.SelectedParticipants().Count);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            // This removes the default title ("Participants") that appears next 
            // to the text on the back button. Only show button without text.
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };

            if (segue.Identifier == "CreateParticipantSegue")
            {
                var viewController = (CreateUserController)segue.DestinationViewController;
                viewController.IsAccountRegistration = false;
            }



            if (segue.Identifier == "SegueToRecordGabber" && Queries.SelectedParticipants().Count == 0)
            {
                Logger.LOG_EVENT_WITH_ACTION("NO_PARTICIPANTS_SELECTED", "TOAST");
                PresentViewController(
                    new Helpers.MessageDialog().BuildErrorMessageDialog(
                        StringResources.participants_ui_validation_noneselected, ""), true, null);
                return;
            }

            if (segue.Identifier == "SegueToRecordGabber" && Queries.SelectedParticipants().Count == 1 && !ConfirmOneParticipant)
            {
                Logger.LOG_EVENT_WITH_ACTION("ONE_PARTICIPANT_MODAL", "DISPLAYED");
                var finishRecordingAlertController = UIAlertController.Create(
                    StringResources.participants_ui_validation_oneselected_title,
                    StringResources.participants_ui_validation_oneselected_message,
                    UIAlertControllerStyle.Alert);

                finishRecordingAlertController.AddAction(
                    UIAlertAction.Create(
                        StringResources.participants_ui_validation_oneselected_cancel,
                        UIAlertActionStyle.Cancel, (_) =>
                        {
                            Logger.LOG_EVENT_WITH_ACTION("ONE_PARTICIPANT_MODAL", "DISMISSED");
                        }
                    )
                );
                finishRecordingAlertController.AddAction(
                    UIAlertAction.Create(StringResources.participants_ui_validation_oneselected_continue,
                                         UIAlertActionStyle.Default, (_) =>
                {
                    Logger.LOG_EVENT_WITH_ACTION("ONE_PARTICIPANT_MODAL", "CONTINUE");
                    ConfirmOneParticipant = true;
                    PerformSegue("SegueToRecordGabber", this);
                }));

                PresentViewController(finishRecordingAlertController, true, null);
            }
        }

        // Revisited this page, i.e. after adding a participant
        [Action("UnwindToParticipantsViewController:")]
        public void UnwindToParticipantsViewController(UIStoryboardSegue segue)
        {
            // Do stuff with added user if needed
            //var sourceController = segue.SourceViewController as CreateUserController;

            //if (sourceController != null)
            //{
            //    Console.WriteLine(sourceController.enteredEmail);
            //}
        }
    }
}