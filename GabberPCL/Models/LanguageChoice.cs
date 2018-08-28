using System;
namespace GabberPCL.Models
{
    public class LanguageChoice
    {
        public string Code { get; set; }
        public string Endonym { get; set; }
        public int Id { get; set; }
        public string Iso_name { get; set; }

        // Example:
        //{
        //    "code": "en",
        //    "endonym": "English",
        //    "id": 1,
        //    "iso_name": "English"
        //}
    }
}
