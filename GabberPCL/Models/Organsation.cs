using SQLite;

namespace GabberPCL.Models
{
    public class Organisation
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}