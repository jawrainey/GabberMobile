using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GabberPCL.Models;

namespace GabberPCL
{
    public static class IFRC_SocietiesManager
    {
        private static bool refreshed;

        public static async Task<List<IFRC_Society>> GetSocieties()
        {
            List<IFRC_Society> cache = Queries.AllSocieties();
            return cache != null && cache.Count > 0 ? cache : await RefreshSocieties();
        }

        public static void RefreshIfNeeded()
        {
            if (!refreshed)
            {
                var suppress = RefreshSocieties();
            }
        }

        private static async Task<List<IFRC_Society>> RefreshSocieties()
        {
            List<IFRC_Society> res = await RestClient.GetSocieties((err) => { });

            if (res != null && res.Count > 0)
            {
                Queries.AddSocieties(res);
                refreshed = true;
                return res;
            }

            return null;
        }
    }
}
