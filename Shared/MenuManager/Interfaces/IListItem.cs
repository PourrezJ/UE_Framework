using System.Collections.Generic;

namespace UE_Shared.MenuManager
{
    public interface IListItem : IMenuItem
    {
        List<string> Items { get; set; }
        int SelectedItem { get; set; }
        bool ExecuteCallbackListChange { get; set; }
        Menu.MenuListCallbackAsync ListItemChangeCallback { get; set; }
    }
}