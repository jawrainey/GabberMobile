using GabberPCL.Interfaces;

namespace GabberPCL.Models
{
    public class LanguageChoice : IProfileOption
    {
        //{
        //    "code": "ar",
        //    "endonym": "العربية",
        //    "id": 4,
        //    "iso_name": "Arabic"
        //}
        public string Code { get; set; }
        public string Endonym { get; set; }
        public int Id { get; set; }
        public string Iso_name { get; set; }

        public int GetId()
        {
            return Id;
        }

        public string GetText()
        {
            return Endonym;
        }
    }
}
