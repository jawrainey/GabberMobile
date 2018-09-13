using CoreGraphics;
using UIKit;

namespace Gabber.iOS
{
    public class Application
    {
        public static CGColor MainColour = UIColor.FromRGB(.886f, .118f, .149f).CGColor;

        static void Main(string[] args) => UIApplication.Main(args, null, "AppDelegate");
    }
}
