using SQLite;

namespace GabberPCL.Models
{
    // This represents a participant, so each new user is a participant
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        // This is the view model and not data model
        public bool Selected { get; set; }
    }
}