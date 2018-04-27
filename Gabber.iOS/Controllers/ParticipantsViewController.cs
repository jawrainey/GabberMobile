using System;
using UIKit;
using Gabber.iOS.ViewSources;
using GabberPCL;
using Foundation;
using GabberPCL.Resources;

namespace Gabber.iOS
{
    public partial class ParticipantsViewController : UIViewController
    {
        bool ConfirmOneParticipant;

        ParticipantsCollectionViewSource participantsViewSource;

        public ParticipantsViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var es = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 36, 70);
            (ParticipantsCollectionView.CollectionViewLayout as UICollectionViewFlowLayout).EstimatedItemSize = es;

            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, a) => {
                PerformSegue("ShowAPVC", this);
            }), true);
            
            // TODO: can define these in storyboard
            var themeColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;

            RecordGabberButton.Layer.BorderWidth = 1.0f;
            RecordGabberButton.Layer.BorderColor = themeColor;

            Title = StringResources.participants_ui_title;

            ParticipantsInstructions.Text = StringResources.participants_ui_instructions;
            RecordGabberButton.SetTitle(StringResources.participants_ui_startrecording_button, UIControlState.Normal);
            NumSelectedParts.Text = string.Format(StringResources.participants_ui_numselected, Queries.SelectedParticipants().Count);
            participantsViewSource = new ParticipantsCollectionViewSource(Queries.AllParticipants());
            participantsViewSource.AddParticipant += (int num) => {
                NumSelectedParts.Text = string.Format(StringResources.participants_ui_numselected, num);
            };
            ParticipantsCollectionView.Source = participantsViewSource;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            participantsViewSource.Rows = Queries.AllParticipants();
            ParticipantsCollectionView.ReloadData();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "SegueToRecordGabber" && Queries.SelectedParticipants().Count == 0)
            {
                PresentViewController(
                    new Helpers.MessageDialog().BuildErrorMessageDialog(
                        StringResources.participants_ui_validation_noneselected, ""), true, null);
                return;
            }

            if (segue.Identifier == "SegueToRecordGabber" && Queries.SelectedParticipants().Count == 1 && !ConfirmOneParticipant)
            {
                var finishRecordingAlertController = UIAlertController.Create(
                    StringResources.participants_ui_validation_oneselected_title, 
                    StringResources.participants_ui_validation_oneselected_message, 
                    UIAlertControllerStyle.Alert);
                
                finishRecordingAlertController.AddAction(
                    UIAlertAction.Create(
                        StringResources.participants_ui_validation_oneselected_cancel, 
                        UIAlertActionStyle.Cancel, (_) => { }
                    )
                );
                finishRecordingAlertController.AddAction(
                    UIAlertAction.Create(StringResources.participants_ui_validation_oneselected_continue, 
                                         UIAlertActionStyle.Default, (_) => 
                {
                    ConfirmOneParticipant = true;
                    PerformSegue("SegueToRecordGabber", this);
                }));

                PresentViewController(finishRecordingAlertController, true, null);
            }

            // This removes the default title ("Participants") that appears next 
            // to the text on the back button. Only show button without text.
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
        }

        // Revisited this page, i.e. after adding a participant
        [Action("UnwindToParticipantsViewController:")]
        public void UnwindToParticipantsViewController(UIStoryboardSegue segue) {}
    }
}