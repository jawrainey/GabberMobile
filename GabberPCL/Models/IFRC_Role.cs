using System;
using System.Collections.Generic;
using GabberPCL.Interfaces;
using GabberPCL.Resources;

namespace GabberPCL.Models
{
    public class IFRC_Role : IProfileOption
    {
        public enum RoleEnum { Volunteer, Intern, Staff, Leadership, External };

        public RoleEnum Enum { get; set; }
        public string LocalisedName { get; set; }

        public int GetId()
        {
            return (int)Enum;
        }

        public string GetText()
        {
            return LocalisedName;
        }

        public static List<IFRC_Role> GetOptions()
        {
            return new List<IFRC_Role>
                {
                    new IFRC_Role
                    {
                        LocalisedName = StringResources.common_ui_forms_role_volunteer,
                        Enum = RoleEnum.Volunteer
                    },
                    new IFRC_Role
                    {
                        LocalisedName = StringResources.common_ui_forms_role_intern,
                        Enum = RoleEnum.Intern
                    },
                    new IFRC_Role
                    {
                        LocalisedName = StringResources.common_ui_forms_role_staff,
                        Enum = RoleEnum.Staff
                    },
                    new IFRC_Role
                    {
                        LocalisedName = StringResources.common_ui_forms_role_leadership,
                        Enum = RoleEnum.Leadership
                    },
                    new IFRC_Role
                    {
                        LocalisedName = StringResources.common_ui_forms_role_external,
                        Enum = RoleEnum.External
                    },
                };
        }
    }
}
