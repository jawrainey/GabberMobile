using System;
using System.Collections.Generic;
using System.Drawing;
using Foundation;
using GabberPCL.Interfaces;
using GabberPCL.Models;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS.ViewSources
{
    public class ProfileOptionPickerViewModel : UIPickerViewModel
    {
        private readonly List<IProfileOption> rows;
        private Action<IProfileOption> selectCallback;
        private readonly string firstItem;
        private int offset;

        public ProfileOptionPickerViewModel(List<IProfileOption> data, string defaultOption = null, Action<IProfileOption> didSelect = null)
        {
            firstItem = defaultOption;
            rows = data;
            selectCallback = didSelect;

            offset = (firstItem == null) ? 0 : 1;
        }

        public bool SelectById(UIPickerView pickerView, int id)
        {
            int index = rows.FindIndex((IProfileOption obj) => obj.GetId() == id);

            pickerView.Select(index + offset, 0, true);

            // found?
            return index != -1;
        }

        public IProfileOption GetChoice(UIPickerView pickerView)
        {
            int row = (int)pickerView.SelectedRowInComponent(0);

            if (row < offset) return null;

            return rows[row - offset];
        }

        [Export("pickerView:didSelectRow:inComponent:")]
        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            IProfileOption choice = null;
            if (row >= offset)
            {
                choice = rows[(int)row - offset];
            }

            selectCallback?.Invoke(choice);
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return rows.Count + offset;
        }

        public override nfloat GetRowHeight(UIPickerView pickerView, nint component)
        {
            return 27f;
        }

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
            UILabel lbl = new UILabel(new RectangleF(0, 0, 130f, 40f));
            lbl.TextColor = UIColor.Black;
            lbl.Font = UIFont.SystemFontOfSize(16f);
            lbl.TextAlignment = UITextAlignment.Center;
            lbl.AdjustsFontSizeToFitWidth = true;
            lbl.Text = (row < offset) ? firstItem : rows[(int)row - offset].GetText();
            return lbl;
        }
    }
}
