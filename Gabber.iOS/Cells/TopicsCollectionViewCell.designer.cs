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
    [Register ("TopicsCollectionViewCell")]
    partial class TopicsCollectionViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView ProjectTopic { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProjectTopic != null) {
                ProjectTopic.Dispose ();
                ProjectTopic = null;
            }
        }
    }
}