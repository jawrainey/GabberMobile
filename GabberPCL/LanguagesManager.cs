using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GabberPCL.Models;

namespace GabberPCL
{
    public static class LanguagesManager
    {
        private static bool refreshed;

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
    }
}
