// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Gabber.iOS
{
    [Register ("ConversationConsent")]
    partial class ConversationConsent
    {
        [Outlet]
        UIKit.UILabel ChooseLanguageTitle { get; set; }


        [Outlet]
        UIKit.UIPickerView LanguagePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ConversationConsentSubmit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ConversationConsentTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ConversationDecisionDes { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint tableViewHeight { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ChooseLanguageTitle != null) {
                ChooseLanguageTitle.Dispose ();
                ChooseLanguageTitle = null;
            }

            if (ConversationConsentSubmit != null) {
                ConversationConsentSubmit.Dispose ();
                ConversationConsentSubmit = null;
            }

            if (ConversationConsentTableView != null) {
                ConversationConsentTableView.Dispose ();
                ConversationConsentTableView = null;
            }

            if (ConversationDecisionDes != null) {
                ConversationDecisionDes.Dispose ();
                ConversationDecisionDes = null;
            }

            if (LanguagePicker != null) {
                LanguagePicker.Dispose ();
                LanguagePicker = null;
            }

            if (tableViewHeight != null) {
                tableViewHeight.Dispose ();
                tableViewHeight = null;
            }
        }
    }
}