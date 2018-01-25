using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace GabberPCL
{
    public static class Queries
    {
        public static List<Participant> AllParticipants() => Session.Connection.Table<Participant>().ToList();
        public static List<Participant> SelectedParticipants() => AllParticipants().FindAll((p) => p.Selected);
    }
}
