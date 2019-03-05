using CoreGraphics;
using UIKit;

namespace Gabber.iOS
{
    public class Application
    {
        public static CGColor MainColour = UIColor.FromRGB(.15f, .65f, .60f).CGColor;

        static void Main(string[] args) => UIApplication.Main(args, null, "AppDelegate");
    }
}
