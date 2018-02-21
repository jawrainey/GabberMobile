using UIKit;

namespace Gabber.iOS.Helpers
{
    public class MessageDialog
    {
        public UIAlertController BuildErrorMessageDialog(string title, string message)
        {
            var finishRecordingAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            finishRecordingAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (_) => { }));
            return finishRecordingAlertController;
        }
    }
}
