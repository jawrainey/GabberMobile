using System;
using System.Collections.Generic;
using GabberPCL.Interfaces;

namespace GabberPCL.Models
{
    public class AgeRange : IProfileOption
    {
        public enum GenderEnum { H21, L21H30, L31H40, L41 };

        public GenderEnum Enum { get; set; }
        public string DisplayName { get; set; }

        public int GetId()
        {
            return (int)Enum;
        }

        public string GetText()
        {
            return DisplayName;
        }

        public static List<AgeRange> GetOptions()
        {
            return new List<AgeRange>
                {
                    new AgeRange
                    {
                        Enum = GenderEnum.H21,
                        DisplayName = "< 21"
                    },
                    new AgeRange
                    {
                        Enum = GenderEnum.L21H30,
                        DisplayName = "21 - 30"
                    },
                    new AgeRange
                    {
                        Enum = GenderEnum.L31H40,
                        DisplayName = "31 - 40"
                    },
                    new AgeRange
                    {
                        Enum = GenderEnum.L41,
                        DisplayName = "41+"
                    }
                };
        }
    }
}
