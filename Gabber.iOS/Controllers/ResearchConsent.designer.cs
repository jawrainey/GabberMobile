// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Gabber.iOS
{
    [Register ("ResearchConsent")]
    partial class ResearchConsent
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ResearchConsentDesc { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ResearchConsentFormDetails { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch ResearchConsentFormSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ResearchConsentSubmit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView ResearchConsentSV { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ResearchConsentTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ResearchConsentDesc != null) {
                ResearchConsentDesc.Dispose ();
                ResearchConsentDesc = null;
            }

            if (ResearchConsentFormDetails != null) {
                ResearchConsentFormDetails.Dispose ();
                ResearchConsentFormDetails = null;
            }

            if (ResearchConsentFormSwitch != null) {
                ResearchConsentFormSwitch.Dispose ();
                ResearchConsentFormSwitch = null;
            }

            if (ResearchConsentSubmit != null) {
                ResearchConsentSubmit.Dispose ();
                ResearchConsentSubmit = null;
            }

            if (ResearchConsentSV != null) {
                ResearchConsentSV.Dispose ();
                ResearchConsentSV = null;
            }

            if (ResearchConsentTitle != null) {
                ResearchConsentTitle.Dispose ();
                ResearchConsentTitle = null;
            }
        }
    }
}