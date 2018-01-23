using Foundation;
using System;
using UIKit;
using GabberPCL;
using Newtonsoft.Json;

namespace Gabber.iOS
{
    public partial class RecordViewController : UIViewController
    {
        public RecordViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            var model = new DatabaseManager(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            var selectedProject = model.ProjectByName(NSUserDefaults.StandardUserDefaults.StringForKey("selectedProject"));
            var topics = selectedProject.prompts;
            Console.WriteLine(JsonConvert.SerializeObject(selectedProject.prompts));
        }
    }
}