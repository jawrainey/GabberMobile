using System;
using GabberPCL.Interfaces;

namespace Gabber.iOS.Helpers
{
    public class PrivatePath : IPrivatePath
    {
        string IPrivatePath.PrivatePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}