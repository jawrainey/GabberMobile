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
    [Register ("ConsentSummary")]
    partial class ConsentSummary
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConsentConversationDesc { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConsentEmbargoDesc { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConsentParticipantsDesc { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConsentResearchDesc { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ConsentSummarySubmit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConsentSummaryTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConsentTitleDesc { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ConsentConversationDesc != null) {
                ConsentConversationDesc.Dispose ();
                ConsentConversationDesc = null;
            }

            if (ConsentEmbargoDesc != null) {
                ConsentEmbargoDesc.Dispose ();
                ConsentEmbargoDesc = null;
            }

            if (ConsentParticipantsDesc != null) {
                ConsentParticipantsDesc.Dispose ();
                ConsentParticipantsDesc = null;
            }

            if (ConsentResearchDesc != null) {
                ConsentResearchDesc.Dispose ();
                ConsentResearchDesc = null;
            }

            if (ConsentSummarySubmit != null) {
                ConsentSummarySubmit.Dispose ();
                ConsentSummarySubmit = null;
            }

            if (ConsentSummaryTitle != null) {
                ConsentSummaryTitle.Dispose ();
                ConsentSummaryTitle = null;
            }

            if (ConsentTitleDesc != null) {
                ConsentTitleDesc.Dispose ();
                ConsentTitleDesc = null;
            }
        }
    }
}