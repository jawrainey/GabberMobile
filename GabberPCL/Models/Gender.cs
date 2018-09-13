using System;
using System.Collections.Generic;
using GabberPCL.Interfaces;
using GabberPCL.Resources;

namespace GabberPCL.Models
{
    public class Gender : IProfileOption
    {
        public enum GenderEnum { Female, Male, Custom, NotSpecified };

        public GenderEnum Enum { get; set; }
        public string LocalisedName { get; set; }
        public string Data { get; set; }

        public int GetId()
        {
            return (int)Enum;
        }

        public string GetText()
        {
            return LocalisedName;
        }

        public static List<Gender> GetOptions()
        {
            return new List<Gender>
                {
                    new Gender
                    {
                        Enum = GenderEnum.Female,
                        LocalisedName = StringResources.common_ui_forms_gender_female
                    },
                    new Gender
                    {
                        Enum = GenderEnum.Male,
                        LocalisedName = StringResources.common_ui_forms_gender_male
                    },
                    new Gender
                    {
                        Enum = GenderEnum.Custom,
                        LocalisedName = StringResources.common_ui_forms_gender_custom
                    },
                    new Gender
                    {
                        Enum = GenderEnum.NotSpecified,
                        LocalisedName = StringResources.common_ui_forms_gender_anon
                    }
                };
        }
    }
}
