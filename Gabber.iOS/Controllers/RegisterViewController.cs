using System;
using System.Text.RegularExpressions;
using Foundation;
using GabberPCL;
using GabberPCL.Resources;
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

            FullNameRegisterTextField.Placeholder = StringResources.register_ui_fullname_label;
            EmailRegisterTextField.Placeholder = StringResources.common_ui_forms_email_label;
            PasswordRegisterTextField.Placeholder = StringResources.common_ui_forms_password_label;
            RegisterUIButton.SetTitle(StringResources.register_ui_submit_button, UIControlState.Normal);

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
            if (string.IsNullOrWhiteSpace(fname))
            {
                ErrorMessageDialog(StringResources.register_ui_fullname_validate_empty);
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                ErrorMessageDialog(StringResources.common_ui_forms_email_validate_empty);
            }
            else if (string.IsNullOrWhiteSpace(passw))
            {
                ErrorMessageDialog(StringResources.common_ui_forms_password_validate_empty);
            }
            else if (!Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                ErrorMessageDialog(StringResources.common_ui_forms_email_validate_invalid);
            }
            else
            {
                RegisterUIButton.Enabled = false;
                var client = new RestClient();
                RegisterActivityIndicator.StartAnimating();
                var response = await client.Register(fname, email, passw);
                RegisterActivityIndicator.StopAnimating();
                RegisterUIButton.Enabled = true;

                if (response.Meta.Success)
                {
                    // TODO: should take us to the verification page ...

                    // Set the root view as ProjectsVC; handled in AppDelegate
                    UIApplication.SharedApplication.Windows[0].RootViewController =
                        UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
                }
                else if (response.Meta.Messages.Count > 0)
                {
                    // Note: errors returned by register are the same as logjn, hence using that for lookup.
                    var err = StringResources.ResourceManager.GetString($"login.api.error.{response.Meta.Messages[0]}");
                    ErrorMessageDialog(err);
                }
            }
        }

        void ErrorMessageDialog(string title)
        {
            var dialog = new Helpers.MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog(title, "");
            PresentViewController(errorDialog, true, null);
        }
    }
}