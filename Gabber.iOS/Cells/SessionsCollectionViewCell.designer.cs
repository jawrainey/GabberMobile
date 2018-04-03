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
    [Register ("SessionsCollectionViewCell")]
    partial class SessionsCollectionViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SessionCreateDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView SessionIsUploaded { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView SessionIsUploadedIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SessionLength { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SessionNumTopics { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SessionParticipants { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SessionProjectTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SessionCreateDate != null) {
                SessionCreateDate.Dispose ();
                SessionCreateDate = null;
            }

            if (SessionIsUploaded != null) {
                SessionIsUploaded.Dispose ();
                SessionIsUploaded = null;
            }

            if (SessionIsUploadedIndicator != null) {
                SessionIsUploadedIndicator.Dispose ();
                SessionIsUploadedIndicator = null;
            }

            if (SessionLength != null) {
                SessionLength.Dispose ();
                SessionLength = null;
            }

            if (SessionNumTopics != null) {
                SessionNumTopics.Dispose ();
                SessionNumTopics = null;
            }

            if (SessionParticipants != null) {
                SessionParticipants.Dispose ();
                SessionParticipants = null;
            }

            if (SessionProjectTitle != null) {
                SessionProjectTitle.Dispose ();
                SessionProjectTitle = null;
            }
        }
    }
}