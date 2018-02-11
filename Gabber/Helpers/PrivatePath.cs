using System;
using GabberPCL.Interfaces;

namespace Gabber.Helpers
{
    public class PrivatePath : IPrivatePath
    {
        string IPrivatePath.PrivatePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}