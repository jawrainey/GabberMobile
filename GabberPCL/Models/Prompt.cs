namespace GabberPCL
{
    public class Prompt
    {
        public string prompt { get; set; }
        public string imageName { get; set; }
        public bool Selected { get; set; }
        public SelectedState SelectionState { get; set; }
        public enum SelectedState { never, previous, current }
	}
}