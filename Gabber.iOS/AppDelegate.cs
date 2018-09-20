using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // If the user is not logged in (hence has not created an account), then show the login view.
            // The Root view (i.e. TabBarController) is shown and set once the user login or registers.
            if (string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey("tokens")))
            {
                // Required to show onboarding in native device language or English if resources dont exist.
                StringResources.Culture = Localize.GetCurrentCultureInfo();
                Window.RootViewController = UIStoryboard.FromName("Main", null).InstantiateViewController("Onboarding");
            }

            // Create here as this method will always get run when opening the app.
            Firebase.Crashlytics.Crashlytics.Configure();
            Firebase.Core.App.Configure();

            // Used by the PCL for database interactions so must be defined early.
            Session.PrivatePath = new PrivatePath();

            // Register the implementation to the global interface within the PCL.
            RestClient.GlobalIO = new DiskIO();
            return true;
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
    }
}