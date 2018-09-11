using System;
namespace GabberPCL.Models
{
    public class AgeRange
    {
        public enum GenderEnum { H21, L21H30, L31H40, L41 };

        public GenderEnum Enum { get; set; }
        public string DisplayName { get; set; }
    }
}
