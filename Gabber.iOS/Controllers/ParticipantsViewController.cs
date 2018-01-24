using System;
using UIKit;
using System.Collections.Generic;
using Gabber.iOS.ViewSources;
using GabberPCL;
using Newtonsoft.Json;
using Foundation;

namespace Gabber.iOS
{
    public partial class ParticipantsViewController : UIViewController
    {
        List<Participant> participants;

        public ParticipantsViewController (IntPtr handle) : base (handle) 
        {
            var known_participants = NSUserDefaults.StandardUserDefaults.StringForKey("participants");

            // The first time a user visits this screen they must view their own details
            if (string.IsNullOrEmpty(known_participants))
            {
                // Note: the user is selected by default
                var itsame = new Participant { Name = "(You)", Email = "TODO_AFTER_LOGIN", Selected = true };
                var __participants = new List<Participant> { itsame };
                // TODO: stored to local storage for now to simplify access as DatabaseManager needs rewritten.
                NSUserDefaults.StandardUserDefaults.SetString(JsonConvert.SerializeObject(__participants), "participants");
                participants = __participants;
            }
            else
            {
                participants = JsonConvert.DeserializeObject<List<Participant>>(known_participants);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ParticipantsCollectionView.Source = new ParticipantsCollectionViewSource(participants);

            // The selection state is mirror the participants on form load, then we can
            // update the UI state in ViewSource and not worry about the row model.
            // HACK: this was required as all rows are false by default, which meant if a participant was
            // selected (such as from a previous use of the app), then the UI would update (via ViewCell),
            // but a double-click in ItemSelected is required to show the DeSelected state.
            for (int i = 0; i < participants.Count; i++)
            {
                if (participants[i].Selected)
                {
                    ParticipantsCollectionView.SelectItem(NSIndexPath.FromRowSection(i, 0), false, 0);
                }
            }
        }
    }
}