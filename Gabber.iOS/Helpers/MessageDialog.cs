using System;
using UIKit;

namespace Gabber.iOS.Helpers
{
    public class MessageDialog
    {
        public UIAlertController BuildErrorMessageDialog(string message)
        {
            var finishRecordingAlertController = UIAlertController.Create(
                "Unable to log in",
                message,
                UIAlertControllerStyle.Alert
            );

            finishRecordingAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (_) => { }));
            return finishRecordingAlertController;
        }
    }
}
