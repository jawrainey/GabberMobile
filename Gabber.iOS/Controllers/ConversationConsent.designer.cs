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
    [Register ("ConversationConsent")]
    partial class ConversationConsent
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConversationConsentContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ConversationConsentSubmit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ConversationConsentTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConversationConsentTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConversationDecisionDes { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConversationDecisionTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ConversationConsentContent != null) {
                ConversationConsentContent.Dispose ();
                ConversationConsentContent = null;
            }

            if (ConversationConsentSubmit != null) {
                ConversationConsentSubmit.Dispose ();
                ConversationConsentSubmit = null;
            }

            if (ConversationConsentTableView != null) {
                ConversationConsentTableView.Dispose ();
                ConversationConsentTableView = null;
            }

            if (ConversationConsentTitle != null) {
                ConversationConsentTitle.Dispose ();
                ConversationConsentTitle = null;
            }

            if (ConversationDecisionDes != null) {
                ConversationDecisionDes.Dispose ();
                ConversationDecisionDes = null;
            }

            if (ConversationDecisionTitle != null) {
                ConversationDecisionTitle.Dispose ();
                ConversationDecisionTitle = null;
            }
        }
    }
}