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
    [Register ("ProjectTableViewPromptCell")]
    partial class ProjectTableViewPromptCell
    {
        [Outlet]
        UIKit.UILabel PromptLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (PromptLabel != null) {
                PromptLabel.Dispose ();
                PromptLabel = null;
            }
        }
    }
}