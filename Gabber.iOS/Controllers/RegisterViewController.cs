using System;
using System.Text.RegularExpressions;
using Foundation;
using GabberPCL;
using GabberPCL.Models;
using Newtonsoft.Json;
using UIKit;

namespace Gabber.iOS
{
    public partial class RegisterViewController : UIViewController
    {
        public RegisterViewController (IntPtr handle) : base (handle) {}

        public static void SetUpViewKeyboardAnimation(UIView view, CoreGraphics.CGRect defaultUI)
        {
            view.AddGestureRecognizer(
                new UITapGestureRecognizer(() => view.EndEditing(true)) { CancelsTouchesInView = false }
            );

            var notification = UIKeyboard.Notifications.ObserveDidShow((sender, args) =>
            {
                var kbSize = ((NSValue)args.Notification.UserInfo[UIKeyboard.FrameBeginUserInfoKey]).RectangleFValue.Size;
                var z = defaultUI;

                UIView.Animate(.4, () => {
                    z.Height += (kbSize.Height);
                    view.Bounds = z;
                    view.LayoutIfNeeded();

                });
            });

            var _notification = UIKeyboard.Notifications.ObserveWillHide((sender, args) => {
                view.Bounds = defaultUI;
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RegisterUIButton.Layer.BorderWidth = .5f;
            RegisterUIButton.Layer.BorderColor = UIColor.Black.CGColor;

            FullNameRegisterTextField.ShouldReturn += NavigateNext;
            EmailRegisterTextField.ShouldReturn += NavigateNext;
            PasswordRegisterTextField.ShouldReturn += NavigateNext;

            SetUpViewKeyboardAnimation(RegisterMasterView, RegisterMasterView.Bounds);
        }

        bool NavigateNext(UITextField _field)
        {
            if (_field.Tag == 0)
            {
                View.ViewWithTag(1).BecomeFirstResponder();
            }
            else if (_field.Tag == 1) 
            {
                View.ViewWithTag(2).BecomeFirstResponder();
            }
            else
            {
                _field.ResignFirstResponder();
                Register(RegisterUIButton);
            }
            return false;
        }

        async partial void Register(UIButton sender)
        {
            var fname = FullNameRegisterTextField.Text;
            var email = EmailRegisterTextField.Text;
            var passw = PasswordRegisterTextField.Text;

            // TODO: Should refactor validation to a helper class as this is shared with LoginVC
            if (string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passw))
            {
                ErrorMessageDialog("All fields are required");
            }
            else if (!Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                ErrorMessageDialog("The email address provided is invalid");
            }
            else
            {
                RegisterUIButton.Enabled = false;
                var client = new RestClient();
                RegisterActivityIndicator.StartAnimating();
                var tokens = await client.Register(fname, email, passw, (message) => ErrorMessageDialog(message));
                RegisterActivityIndicator.StopAnimating();
                RegisterUIButton.Enabled = true;

                if (!string.IsNullOrEmpty(tokens.Access))
                {
                    NSUserDefaults.StandardUserDefaults.SetString(JsonConvert.SerializeObject(tokens), "ActiveUserTokens");
                    NSUserDefaults.StandardUserDefaults.SetString(email, "Username");
                    // This user is new, so needs to be created locally.
                    Session.Connection.Insert(new User
                    {
                        Name = fname + " (You)",
                        Email = email,
                        Selected = true
                    });
                    // Set the root view as ProjectsVC; handled in AppDelegate
                    UIApplication.SharedApplication.Windows[0].RootViewController =
                        UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
                }
            }
        }

        void ErrorMessageDialog(string message)
        {
            var dialog = new Helpers.MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog("Unable to register", message);
            PresentViewController(errorDialog, true, null);
        }
    }
}