using SQLite;

namespace GabberPCL.Models
{
    public class Annotation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string SessionID { get; set; }
        // TODO: this should really be the TopicID
        public string Topic { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}