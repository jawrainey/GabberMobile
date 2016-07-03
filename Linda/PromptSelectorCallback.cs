using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;

namespace Linda
{
	public class PromptSelectorCallback : ItemTouchHelper.SimpleCallback
	{
		readonly RecyclerView _promptLayout;

		public PromptSelectorCallback(int dragDirs, int swipeDirs, RecyclerView promptLayout) : base(dragDirs, swipeDirs)
		{
			_promptLayout = promptLayout;
		}

		public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
		{
			return false;
		}

		public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
		{
			int position = viewHolder.AdapterPosition;
			// Adapter is zero indexed whilst layout manager is not.
			int itemCount = _promptLayout.GetLayoutManager().ItemCount - 1;
			// Change the position based on direction accounting for start/end positions to reset loop.
			if (direction == ItemTouchHelper.Left) position = (position == itemCount) ? 0 : position + 1;
			else position = (position == 0) ? itemCount : position - 1;
			_promptLayout.ScrollToPosition(position);
		}
	}
}