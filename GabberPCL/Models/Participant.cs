using SQLite;

namespace GabberPCL
{
	public class Participant
	{
        [PrimaryKey]
        public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Age { get; set; }
		public string Gender { get; set; }
		public string Photo { get; set; }
		public string Needs { get; set; }
        // This is the view model and not data model
        public bool Selected { get; set; }
	}
}