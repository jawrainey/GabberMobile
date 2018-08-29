// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Gabber.iOS
{
	[Register ("ConversationConsent")]
	partial class ConversationConsent
	{
		[Outlet]
		UIKit.UILabel ChooseLanguageTitle { get; set; }

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

		[Outlet]
		UIKit.UIPickerView LanguagePicker { get; set; }
		
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

			if (ChooseLanguageTitle != null) {
				ChooseLanguageTitle.Dispose ();
				ChooseLanguageTitle = null;
			}

			if (LanguagePicker != null) {
				LanguagePicker.Dispose ();
				LanguagePicker = null;
			}
		}
	}
}
