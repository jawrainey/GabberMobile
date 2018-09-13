using System.Globalization;
using System.Threading;
using Foundation;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.iOS.Helpers
{
    public static class Localize
    {
        public static void SetLocale (CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        public static void SetLayoutDirectionByPreference()
        {
            var info = StringResources.Culture;
            SetLocale(info);
            NSUserDefaults.StandardUserDefaults.SetValueForKey(
                NSArray.FromStrings(info.TwoLetterISOLanguageName), 
                new NSString("AppleLanguages"));
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public static CultureInfo GetCurrentCultureInfo ()
        {
            var netLanguage = "en";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages [0];
                netLanguage = IOSToDotnetLanguage(pref);
            }

            CultureInfo ci = null;
            try 
            {
                ci = new System.Globalization.CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException e1)
            {
                // iOS locale not valid .NET culture (eg. "en-ES" : English in Spain)
                // fallback to first characters, in this case "en"
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    ci = new System.Globalization.CultureInfo(fallback);
                }
                catch (CultureNotFoundException e2)
                {
                    ci = new System.Globalization.CultureInfo("en");
                }
            }
                
            return ci;
        }

        private static string IOSToDotnetLanguage(string iOSLanguage)
        {
            var netLanguage = iOSLanguage; 

            switch (iOSLanguage)
            {
                case "ms-MY":   // "Malaysian (Malaysia)" not supported .NET culture
                case "ms-SG":   // "Malaysian (Singapore)" not supported .NET culture
                    netLanguage = "ms"; // closest supported
                    break;
                case "gsw-CH":  // "Schwiizertüütsch (Swiss German)" not supported .NET culture
                    netLanguage = "de-CH"; // closest supported
                    break;
            }

            return netLanguage;
        }
        static string ToDotnetFallbackLanguage (PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode; // use the first part of the identifier (two chars, usually);

            switch (platCulture.LanguageCode)
            {
                case "pt":
                    netLanguage = "pt-PT"; // fallback to Portuguese (Portugal)
                    break;
                case "gsw":
                    netLanguage = "de-CH"; // equivalent to German (Switzerland) for this app
                    break;
            }
            return netLanguage;
        }
    }
}