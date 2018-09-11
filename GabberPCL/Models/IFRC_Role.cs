using System;
namespace GabberPCL.Models
{
    public class IFRC_Role
    {
        public enum RoleEnum { Volunteer, Intern, Staff, Leadership, External };

        public RoleEnum Enum { get; set; }
        public string LocalisedName { get; set; }
    }
}
