using System;
using System.Collections.Generic;
using System.Linq;
using GabberPCL.Interfaces;
using Newtonsoft.Json;

namespace GabberPCL.Models
{
    public class IFRC_Society : IProfileOption
    {
        private static List<IFRC_Society> loaded;

        public int Id { get; set; }
        public string Name { get; set; }

        public int GetId()
        {
            return Id;
        }

        public string GetText()
        {
            return Name;
        }

        public static List<IFRC_Society> GetOptions()
        {
            if (loaded != null) return loaded;

            loaded = new List<IFRC_Society>
            {
                new IFRC_Society {
                    Id= 1,
                    Name= "Afghan Red Crescent"
                },
                new IFRC_Society {
                    Id= 2,
                    Name= "Albanian Red Cross"
                },
                new IFRC_Society {
                    Id= 3,
                    Name= "Algerian Red Crescent"
                },
                new IFRC_Society {
                    Id= 4,
                    Name= "American Red Cross"
                },
                new IFRC_Society {
                    Id= 5,
                    Name= "Andorran Red Cross"
                },
                new IFRC_Society {
                    Id= 6,
                    Name= "Angola Red Cross"
                },
                new IFRC_Society {
                    Id= 7,
                    Name= "Antigua and Barbuda Red Cross"
                },
                new IFRC_Society {
                    Id= 8,
                    Name= "Argentine Red Cross"
                },
                new IFRC_Society {
                    Id= 9,
                    Name= "Armenian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 10,
                    Name= "Australian Red Cross"
                },
                new IFRC_Society {
                    Id= 11,
                    Name= "Austrian Red Cross"
                },
                new IFRC_Society {
                    Id= 12,
                    Name= "Bahrain Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 13,
                    Name= "Bangladesh Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 14,
                    Name= "Baphalali Swaziland Red Cross Society"
                },
                new IFRC_Society {
                    Id= 15,
                    Name= "Belarus Red Cross"
                },
                new IFRC_Society {
                    Id= 16,
                    Name= "Belgian Red Cross"
                },
                new IFRC_Society {
                    Id= 17,
                    Name= "Belize Red Cross Society"
                },
                new IFRC_Society {
                    Id= 18,
                    Name= "Bolivian Red Cross"
                },
                new IFRC_Society {
                    Id= 19,
                    Name= "Botswana Red Cross Society"
                },
                new IFRC_Society {
                    Id= 20,
                    Name= "Brazilian Red Cross"
                },
                new IFRC_Society {
                    Id= 21,
                    Name= "British Red Cross"
                },
                new IFRC_Society {
                    Id= 22,
                    Name= "Brunei Darussalam Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 23,
                    Name= "Bulgarian Red Cross"
                },
                new IFRC_Society {
                    Id= 24,
                    Name= "Burkinabe Red Cross Society"
                },
                new IFRC_Society {
                    Id= 25,
                    Name= "Burundi Red Cross"
                },
                new IFRC_Society {
                    Id= 26,
                    Name= "Cambodian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 27,
                    Name= "Cameroon Red Cross Society"
                },
                new IFRC_Society {
                    Id= 28,
                    Name= "Central African Red Cross Society"
                },
                new IFRC_Society {
                    Id= 29,
                    Name= "Chilean Red Cross"
                },
                new IFRC_Society {
                    Id= 30,
                    Name= "Colombian Red Cross"
                },
                new IFRC_Society {
                    Id= 31,
                    Name= "Congolese Red Cross"
                },
                new IFRC_Society {
                    Id= 32,
                    Name= "Cook Islands Red Cross Society"
                },
                new IFRC_Society {
                    Id= 33,
                    Name= "Costa Rican Red Cross"
                },
                new IFRC_Society {
                    Id= 34,
                    Name= "Croatian Red Cross"
                },
                new IFRC_Society {
                    Id= 35,
                    Name= "Cuban Red Cross"
                },
                new IFRC_Society {
                    Id= 36,
                    Name= "Cyprus Red Cross Society"
                },
                new IFRC_Society {
                    Id= 37,
                    Name= "Czech Red Cross"
                },
                new IFRC_Society {
                    Id= 38,
                    Name= "Danish Red Cross"
                },
                new IFRC_Society {
                    Id= 39,
                    Name= "Dominica Red Cross Society"
                },
                new IFRC_Society {
                    Id= 40,
                    Name= "Dominican Red Cross"
                },
                new IFRC_Society {
                    Id= 41,
                    Name= "Ecuadorian Red Cross"
                },
                new IFRC_Society {
                    Id= 42,
                    Name= "Egyptian Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 43,
                    Name= "Estonian Red Cross"
                },
                new IFRC_Society {
                    Id= 44,
                    Name= "Ethiopian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 45,
                    Name= "Fiji Red Cross Society"
                },
                new IFRC_Society {
                    Id= 46,
                    Name= "Finnish Red Cross"
                },
                new IFRC_Society {
                    Id= 47,
                    Name= "French Red Cross"
                },
                new IFRC_Society {
                    Id= 48,
                    Name= "Gabonese Red Cross Society"
                },
                new IFRC_Society {
                    Id= 49,
                    Name= "German Red Cross"
                },
                new IFRC_Society {
                    Id= 50,
                    Name= "Ghana Red Cross Society"
                },
                new IFRC_Society {
                    Id= 51,
                    Name= "Grenada Red Cross Society"
                },
                new IFRC_Society {
                    Id= 52,
                    Name= "Guatemalan Red Cross"
                },
                new IFRC_Society {
                    Id= 53,
                    Name= "Haiti Red Cross Society"
                },
                new IFRC_Society {
                    Id= 54,
                    Name= "Hellenic Red Cross"
                },
                new IFRC_Society {
                    Id= 55,
                    Name= "Honduran Red Cross"
                },
                new IFRC_Society {
                    Id= 56,
                    Name= "Hungarian Red Cross"
                },
                new IFRC_Society {
                    Id= 57,
                    Name= "Icelandic Red Cross"
                },
                new IFRC_Society {
                    Id= 58,
                    Name= "Indian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 59,
                    Name= "Indonesian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 60,
                    Name= "Iraqi Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 61,
                    Name= "Irish Red Cross Society"
                },
                new IFRC_Society {
                    Id= 62,
                    Name= "Israel - Magen David Adom in Israel"
                },
                new IFRC_Society {
                    Id= 63,
                    Name= "Italian Red Cross"
                },
                new IFRC_Society {
                    Id= 64,
                    Name= "Jamaica Red Cross"
                },
                new IFRC_Society {
                    Id= 65,
                    Name= "Japanese Red Cross Society"
                },
                new IFRC_Society {
                    Id= 66,
                    Name= "Jordan National Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 67,
                    Name= "Kazakh Red Crescent"
                },
                new IFRC_Society {
                    Id= 68,
                    Name= "Kenya Red Cross Society"
                },
                new IFRC_Society {
                    Id= 69,
                    Name= "Kiribati Red Cross Society"
                },
                new IFRC_Society {
                    Id= 70,
                    Name= "Kuwait Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 71,
                    Name= "Lao Red Cross"
                },
                new IFRC_Society {
                    Id= 72,
                    Name= "Latvian Red Cross"
                },
                new IFRC_Society {
                    Id= 73,
                    Name= "Lebanese Red Cross"
                },
                new IFRC_Society {
                    Id= 74,
                    Name= "Lesotho Red Cross Society"
                },
                new IFRC_Society {
                    Id= 75,
                    Name= "Liberian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 76,
                    Name= "Libyan Red Crescent"
                },
                new IFRC_Society {
                    Id= 77,
                    Name= "Liechtenstein Red Cross"
                },
                new IFRC_Society {
                    Id= 78,
                    Name= "Lithuanian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 79,
                    Name= "Luxembourg Red Cross"
                },
                new IFRC_Society {
                    Id= 80,
                    Name= "Malagasy Red Cross Society"
                },
                new IFRC_Society {
                    Id= 81,
                    Name= "Malawi Red Cross Society"
                },
                new IFRC_Society {
                    Id= 82,
                    Name= "Malaysian Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 83,
                    Name= "Maldivian Red Crescent"
                },
                new IFRC_Society {
                    Id= 84,
                    Name= "Mali Red Cross"
                },
                new IFRC_Society {
                    Id= 85,
                    Name= "Malta Red Cross Society"
                },
                new IFRC_Society {
                    Id= 86,
                    Name= "Mauritanian Red Crescent"
                },
                new IFRC_Society {
                    Id= 87,
                    Name= "Mauritius Red Cross Society"
                },
                new IFRC_Society {
                    Id= 88,
                    Name= "Mexican Red Cross"
                },
                new IFRC_Society {
                    Id= 89,
                    Name= "Micronesia Red Cross"
                },
                new IFRC_Society {
                    Id= 90,
                    Name= "Mongolian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 91,
                    Name= "Moroccan Red Crescent"
                },
                new IFRC_Society {
                    Id= 92,
                    Name= "Mozambique Red Cross Society"
                },
                new IFRC_Society {
                    Id= 93,
                    Name= "Myanmar Red Cross Society"
                },
                new IFRC_Society {
                    Id= 94,
                    Name= "Namibia Red Cross"
                },
                new IFRC_Society {
                    Id= 95,
                    Name= "Nepal Red Cross Society"
                },
                new IFRC_Society {
                    Id= 96,
                    Name= "New Zealand Red Cross"
                },
                new IFRC_Society {
                    Id= 97,
                    Name= "Nicaraguan Red Cross"
                },
                new IFRC_Society {
                    Id= 98,
                    Name= "Nigerian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 99,
                    Name= "Norwegian Red Cross"
                },
                new IFRC_Society {
                    Id= 100,
                    Name= "Pakistan Red Crescent"
                },
                new IFRC_Society {
                    Id= 101,
                    Name= "Palau Red Cross Society"
                },
                new IFRC_Society {
                    Id= 102,
                    Name= "Papua New Guinea Red Cross Society"
                },
                new IFRC_Society {
                    Id= 103,
                    Name= "Paraguayan Red Cross"
                },
                new IFRC_Society {
                    Id= 104,
                    Name= "Peruvian Red Cross"
                },
                new IFRC_Society {
                    Id= 105,
                    Name= "Philippine Red Cross"
                },
                new IFRC_Society {
                    Id= 106,
                    Name= "Polish Red Cross"
                },
                new IFRC_Society {
                    Id= 107,
                    Name= "Portuguese Red Cross"
                },
                new IFRC_Society {
                    Id= 108,
                    Name= "Qatar Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 109,
                    Name= "Azerbaijan Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 110,
                    Name= "Djibouti Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 111,
                    Name= "Kyrgyzstan Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 112,
                    Name= "Tajikistan Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 113,
                    Name= "Islamic Republic of Iran Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 114,
                    Name= "United Arab Emirates Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 115,
                    Name= "Turkmenistan Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 116,
                    Name= "Uzbekistan Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 117,
                    Name= "Benin Red Cross"
                },
                new IFRC_Society {
                    Id= 118,
                    Name= "Cape Verde Red Cross"
                },
                new IFRC_Society {
                    Id= 119,
                    Name= "Chad Red Cross"
                },
                new IFRC_Society {
                    Id= 120,
                    Name= "Monaco Red Cross"
                },
                new IFRC_Society {
                    Id= 121,
                    Name= "Montenegro Red Cross"
                },
                new IFRC_Society {
                    Id= 122,
                    Name= "Democratic Republic of the Congo Red Cross"
                },
                new IFRC_Society {
                    Id= 123,
                    Name= "Republic of San Marino Red Cross"
                },
                new IFRC_Society {
                    Id= 124,
                    Name= "China Red Cross Society"
                },
                new IFRC_Society {
                    Id= 125,
                    Name= "Côte d'Ivoire Red Cross Society"
                },
                new IFRC_Society {
                    Id= 126,
                    Name= "Georgia Red Cross Society"
                },
                new IFRC_Society {
                    Id= 127,
                    Name= "Guinea Red Cross Society"
                },
                new IFRC_Society {
                    Id= 128,
                    Name= "Guinea-Bissau Red Cross Society"
                },
                new IFRC_Society {
                    Id= 129,
                    Name= "Niger Red Cross Society"
                },
                new IFRC_Society {
                    Id= 130,
                    Name= "Panama Red Cross Society"
                },
                new IFRC_Society {
                    Id= 131,
                    Name= "Democratic People's Republic of Korea Red Cross Society"
                },
                new IFRC_Society {
                    Id= 132,
                    Name= "Republic of Moldova Red Cross Society"
                },
                new IFRC_Society {
                    Id= 133,
                    Name= "Romanian Red Cross"
                },
                new IFRC_Society {
                    Id= 134,
                    Name= "Rwandan Red Cross"
                },
                new IFRC_Society {
                    Id= 135,
                    Name= "Saint Kitts and Nevis Red Cross Society"
                },
                new IFRC_Society {
                    Id= 136,
                    Name= "Saint Lucia Red Cross"
                },
                new IFRC_Society {
                    Id= 137,
                    Name= "Saint Vincent and the Grenadines Red Cross"
                },
                new IFRC_Society {
                    Id= 138,
                    Name= "Salvadorean Red Cross Society"
                },
                new IFRC_Society {
                    Id= 139,
                    Name= "Samoa Red Cross Society"
                },
                new IFRC_Society {
                    Id= 140,
                    Name= "Sao Tome and Principe Red Cross"
                },
                new IFRC_Society {
                    Id= 141,
                    Name= "Saudi Red Crescent Authority"
                },
                new IFRC_Society {
                    Id= 142,
                    Name= "Senegalese Red Cross Society"
                },
                new IFRC_Society {
                    Id= 143,
                    Name= "Seychelles Red Cross Society"
                },
                new IFRC_Society {
                    Id= 144,
                    Name= "Sierra Leone Red Cross Society"
                },
                new IFRC_Society {
                    Id= 145,
                    Name= "Singapore Red Cross Society"
                },
                new IFRC_Society {
                    Id= 146,
                    Name= "Slovak Red Cross"
                },
                new IFRC_Society {
                    Id= 147,
                    Name= "Slovenian Red Cross"
                },
                new IFRC_Society {
                    Id= 148,
                    Name= "Somali Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 149,
                    Name= "South Sudan Red Cross"
                },
                new IFRC_Society {
                    Id= 150,
                    Name= "Spanish Red Cross"
                },
                new IFRC_Society {
                    Id= 151,
                    Name= "Sri Lanka Red Cross Society"
                },
                new IFRC_Society {
                    Id= 152,
                    Name= "Suriname Red Cross"
                },
                new IFRC_Society {
                    Id= 153,
                    Name= "Swedish Red Cross"
                },
                new IFRC_Society {
                    Id= 154,
                    Name= "Swiss Red Cross"
                },
                new IFRC_Society {
                    Id= 155,
                    Name= "Syrian Arab Red Crescent"
                },
                new IFRC_Society {
                    Id= 156,
                    Name= "Tanzania Red Cross National Society"
                },
                new IFRC_Society {
                    Id= 157,
                    Name= "Bahamas Red Cross Society"
                },
                new IFRC_Society {
                    Id= 158,
                    Name= "Barbados Red Cross Society"
                },
                new IFRC_Society {
                    Id= 159,
                    Name= "Canadian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 160,
                    Name= "Comoros Red Crescent"
                },
                new IFRC_Society {
                    Id= 161,
                    Name= "Gambia Red Cross Society"
                },
                new IFRC_Society {
                    Id= 162,
                    Name= "Guyana Red Cross Society"
                },
                new IFRC_Society {
                    Id= 163,
                    Name= "Netherlands Red Cross"
                },
                new IFRC_Society {
                    Id= 164,
                    Name= "Palestine Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 165,
                    Name= "Serbia Red Cross"
                },
                new IFRC_Society {
                    Id= 166,
                    Name= "Former Yugoslav Republic of Macedonia Red Cross"
                },
                new IFRC_Society {
                    Id= 167,
                    Name= "Bosnia and Herzegovina Red Cross Society"
                },
                new IFRC_Society {
                    Id= 168,
                    Name= "Republic of Korea National Red Cross"
                },
                new IFRC_Society {
                    Id= 169,
                    Name= "Russian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 170,
                    Name= "Solomon Islands Red Cross"
                },
                new IFRC_Society {
                    Id= 171,
                    Name= "South African Red Cross Society"
                },
                new IFRC_Society {
                    Id= 172,
                    Name= "Sudanese Red Crescent"
                },
                new IFRC_Society {
                    Id= 173,
                    Name= "Thai Red Cross Society"
                },
                new IFRC_Society {
                    Id= 174,
                    Name= "Trinidad and Tobago Red Cross Society"
                },
                new IFRC_Society {
                    Id= 175,
                    Name= "Uganda Red Cross Society"
                },
                new IFRC_Society {
                    Id= 176,
                    Name= "Timor-Leste Red Cross Society"
                },
                new IFRC_Society {
                    Id= 177,
                    Name= "Togolese Red Cross"
                },
                new IFRC_Society {
                    Id= 178,
                    Name= "Tonga Red Cross Society"
                },
                new IFRC_Society {
                    Id= 179,
                    Name= "Tunisian Red Crescent"
                },
                new IFRC_Society {
                    Id= 180,
                    Name= "Turkish Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 181,
                    Name= "Tuvalu Red Cross Society"
                },
                new IFRC_Society {
                    Id= 182,
                    Name= "Ukrainian Red Cross Society"
                },
                new IFRC_Society {
                    Id= 183,
                    Name= "Uruguayan Red Cross"
                },
                new IFRC_Society {
                    Id= 184,
                    Name= "Vanuatu Red Cross Society"
                },
                new IFRC_Society {
                    Id= 185,
                    Name= "Venezuelan Red Cross"
                },
                new IFRC_Society {
                    Id= 186,
                    Name= "Vietnam Red Cross Society"
                },
                new IFRC_Society {
                    Id= 187,
                    Name= "Yemen Red Crescent Society"
                },
                new IFRC_Society {
                    Id= 188,
                    Name= "Zambia Red Cross Society"
                },
                new IFRC_Society {
                    Id= 189,
                    Name= "Zimbabwe Red Cross Society"
                }
            };

            loaded = loaded.OrderBy((IFRC_Society arg) => arg.Name).ToList();

            loaded.Add(new IFRC_Society
            {
                Id = 190,
                Name = "Was your society not listed? Select this and we will be in touch."
            });

            return loaded;
        }
    }
}
