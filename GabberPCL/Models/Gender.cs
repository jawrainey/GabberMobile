using System;
namespace GabberPCL.Models
{
    public class Gender
    {
        public enum GenderEnum { Female, Male, Custom, NotSpecified };

        public GenderEnum Enum { get; set; }
        public string LocalisedName { get; set; }
        public string Data { get; set; }
    }
}
