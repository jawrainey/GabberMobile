using SQLite;

namespace GabberPCL
{
	// As a request is made each time, simplify by storing as JSON.
	public class ProjectsAsJSON
	{
		[PrimaryKey]
		public int Id { get; set; }
		public string Json { get; set; }
	}
}