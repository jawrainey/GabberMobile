using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL;
using GabberPCL.Resources;
using UIKit;
using UserNotifications;
using Firebase.CloudMessaging;
using System;
using SafariServices;

namespace Gabber.iOS
{
#pragma warning disable XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        public class UserInfoEventArgs : EventArgs
        {
            public NSDictionary UserInfo { get; private set; }
            public MessageType MessageType { get; private set; }

            public UserInfoEventArgs(NSDictionary userInfo, MessageType messageType)
            {
                UserInfo = userInfo;
                MessageType = messageType;
            }
        }

        public enum MessageType
        {
            Notification,
            Data
        }

        public event EventHandler<UserInfoEventArgs> MessageReceived;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UIApplication.SharedApplication.IdleTimerDisabled = true;
            // If the user is not logged in (hence has not created an account), then show the login view.
            // The Root view (i.e. TabBarController) is shown and set once the user login or registers.
            if (string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey("tokens")))
            {
                // Required to show onboarding in native device language or English if resources dont exist.
                StringResources.Culture = Localize.GetCurrentCultureInfo();
                Window.RootViewController = UIStoryboard.FromName("Main", null).InstantiateViewController("Onboarding");
            }

            // Used by the PCL for database interactions so must be defined early.
            Session.PrivatePath = new PrivatePath();
            // Register the implementation to the global interface within the PCL.
            RestClient.GlobalIO = new DiskIO();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.Delegate = this;

                // Request notification permissions from the user
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) => { });
            }
            else
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Sound |
                               UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(notificationSettings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            Messaging.SharedInstance.Delegate = this;
            Messaging.SharedInstance.ShouldEstablishDirectChannel = true;

            // Create here as this method will always get run when opening the app.
            Firebase.Crashlytics.Crashlytics.Configure();
            Firebase.Core.App.Configure();

            return true;
        }

        public override void DidReceiveRemoteNotification(
            UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Handle Notification messages in the background AND foreground. Handles Data messages for iOS 9 and below.
            HandleMessage(userInfo);
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter c, UNNotificationResponse r, Action handler)
        {
            // Handle notification messages after display notification is tapped by the user.
            OpenURLFromNotificationIfExists(r.Notification.Request.Content.UserInfo);
            handler();
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(
            UNUserNotificationCenter c, UNNotification n, Action<UNNotificationPresentationOptions> handler)
        {
            // Receive displayed notifications for iOS 10 devices. Handle incoming notification messages while app is in the foreground.
            handler(UNNotificationPresentationOptions.Alert);
        }

        public void HandleMessage(NSDictionary message)
        {
            if (MessageReceived == null) return;
            var messageType = message.ContainsKey(new NSString("aps")) ? MessageType.Notification : MessageType.Data;
            MessageReceived(this, new UserInfoEventArgs(message, messageType));
        }

        public static void ShowMessage(string title, string message, UIViewController fromViewController, Action actionForOk = null)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(StringResources.common_comms_ok, UIAlertActionStyle.Default, (obj) => actionForOk?.Invoke()))
            fromViewController.PresentViewController(alert, true, null);
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message)
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive.
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

		public override bool ContinueUserActivity(UIApplication a, NSUserActivity ua, UIApplicationRestorationHandler h) => OpenVerify(ua.WebPageUrl);
		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options) => OpenVerify(url);

    bool OpenVerify(NSUrl url)
    {
        if (!string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey("tokens"))) return true;
        NSUserDefaults.StandardUserDefaults.SetURL(url, "VERIFY_URL");
        Window.RootViewController = UIStoryboard.FromName("Main", null).InstantiateViewController("RegisterVerifying");
        return true;
    }

    void OpenURLFromNotificationIfExists(NSDictionary messageFromNotif)
    {
        if (messageFromNotif.ContainsKey(new NSString("url")))
        {
            var url = messageFromNotif.ValueForKey(new NSString("url")).ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                var root = UIApplication.SharedApplication.KeyWindow.RootViewController;
                root.PresentViewControllerAsync(new SFSafariViewController(new NSUrl(url)), true);
            }
        }
    }
  }
#pragma warning restore XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
}
