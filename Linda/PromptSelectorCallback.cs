using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Graphics;
using Android.Widget;

namespace Linda
{
	public class PromptSelectorCallback : ItemTouchHelper.SimpleCallback
	{
		public PromptSelectorCallback(int dragDirs, int swipeDirs) : base(dragDirs, swipeDirs) {}

		public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
		{
			return false;
		}

		public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
		{
			int position = viewHolder.AdapterPosition;
			// Rather messy, but tidier than having an unecessary member variable.
			var rv = ((RecyclerView)viewHolder.ItemView.Parent);
			// Adapter is zero indexed whilst layout manager is not.
			int itemCount = rv.GetLayoutManager().ItemCount - 1;
			// Change the position based on direction accounting for start/end positions to reset loop.
			if (direction == ItemTouchHelper.Left) position = (position == itemCount) ? 0 : position + 1;
			else position = (position == 0) ? itemCount : position - 1;
			// Moves to the position of the element in charge
			rv.ScrollToPosition(position);
		}
	}
}