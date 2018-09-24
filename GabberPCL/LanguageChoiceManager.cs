using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GabberPCL.Models;

namespace GabberPCL
{
    public static class LanguageChoiceManager
    {
        private static bool refreshed;

        public static async Task<LanguageChoice> GetUserLanguage()
        {
            List<LanguageChoice> allLangs = await GetLanguageChoices();
            return allLangs.Find((lang) => lang.Id == Session.ActiveUser.Lang);
        }

        public static async Task<LanguageChoice> GetLanguageFromCode(string code)
        {
            List<LanguageChoice> allLangs = await GetLanguageChoices();
            return allLangs.Find((lang) => lang.Code == code);
        }

        public static async Task<List<LanguageChoice>> GetLanguageChoices()
        {
            List<LanguageChoice> cache = Queries.AllLanguages();
            return cache != null && cache.Count > 0 ? cache : await RefreshLanguages();
        }

        public static void RefreshIfNeeded()
        {
            if (!refreshed)
            {
                var suppress = RefreshLanguages();
            }
        }

        private static async Task<List<LanguageChoice>> RefreshLanguages()
        {
            List<LanguageChoice> res = await RestClient.GetLanguages((err) => { });

            if (res != null && res.Count > 0)
            {
                Queries.AddLanguages(res);
                refreshed = true;
                return res;
            }

            return null;
        }

        public static Content ContentByLanguage(Project project, int requestedId = -1)
        {
            LanguageChoice thisLang;

            if (requestedId != -1)
            {
                thisLang = (GetLanguageChoices().Result).Find(lang => lang.Id == requestedId);
            }
            else
            {
                thisLang = GetUserLanguage().Result;
            }

            var content = project.Content.FirstOrDefault((k) => k.Key == thisLang.Code);

            // Determine if the language above is used
            if (content.Key == null)
            {
                // If the Application Language does not match the project language, then we will use
                // the default selected when creating a project.
                var lang = Queries.AllLanguages().FirstOrDefault((l) => l.Id == project.IsDefaultLang);
                // We should use the default!
                content = project.Content.FirstOrDefault((k) => k.Key == lang.Code);
            }
            return content.Value;
        }
    }
}
