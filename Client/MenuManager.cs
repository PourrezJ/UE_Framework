using System;
using System.Collections.Generic;
using UIMenu = MenuAPI.Menu;
using Menu = UE_Shared.MenuManager.Menu;
using Newtonsoft.Json;
using UE_Shared.MenuManager;
using MenuAPI;
using MenuItem = UE_Shared.MenuManager.MenuItem;
using UE_Shared;

namespace UE_Client
{
    public static class MenuManager
    {
        #region Variables
        private static Menu menuData;
        private static UIMenu uiMenu;

        private static List<MenuItem> listItem;

        private const int ResetTime = 250;
        private static bool _keyStatus = true;
        #endregion

        #region Constructor
        internal static void Init()
        {
            GameMode.RegisterEventHandler("MenuManager_OpenMenu", new Action<string>(OpenMenuManager));
            GameMode.RegisterEventHandler("MenuManager_CloseMenu", new Action(CloseMenu));
        }
        #endregion

        #region Menu
        private static void OpenMenuManager(string data)
        {
            menuData = JsonConvert.DeserializeObject<Menu>(data, new MenuConverter(typeof(Menu)));
            uiMenu = new UIMenu(!string.IsNullOrEmpty(menuData.Title) ? menuData.Title : " ", menuData.SubTitle);
            

            if (menuData.BannerSprite != null)
                uiMenu.HeaderTexture = new KeyValuePair<string, string>(menuData.BannerSprite.Dict, menuData.BannerSprite.Name);
            
            uiMenu.OnMenuClose += (uimenu) =>
            {
                if (!menuData.BackCloseMenu)
                {
                    uiMenu.Visible = true;
                    GameMode.TriggerServerEvent("MenuManager_BackKey");
                }
                else if (CanClose())
                {
                    Logger.Info("close menu");
                    CloseMenu();
                }
            };

            for (int i = 0; i < menuData.Items.Count; i++)
            {
                MenuAPI.MenuItem menuItem = new MenuAPI.MenuItem(menuData.Items[i]?.Text ?? "", menuData.Items[i]?.Description ?? "");

                if (menuData.Items[i].Type == MenuItemType.MenuItem || menuData.Items[i].Type == MenuItemType.ColoredItem)
                {
                    /*
                    if (menuData.Items[i].RightBadge != null)
                        menuItem.SetRightBadge(menuData.Items[i].RightBadge.Value);

                    if (menuData.Items[i].LeftBadge != null)
                        menuItem.SetLeftBadge(menuData.Items[i].RightBadge.Value);

                    if (menuData.Items[i].RightLabel != null)
                        menuItem.SetRightLabel(menuData.Items[i].RightLabel);*/
                }

                else if (menuData.Items[i].Type == MenuItemType.CheckboxItem)
                {
                    menuItem = new MenuAPI.MenuCheckboxItem(menuData.Items[i].Text, menuData.Items[i].Description, ((CheckboxItem)menuData.Items[i]).Checked);
                }
                else if (menuData.Items[i].Type == MenuItemType.ListItem)
                {
                    menuItem = new MenuAPI.MenuListItem(menuData.Items[i].Text, ((ListItem)menuData.Items[i]).Items, ((ListItem)menuData.Items[i]).SelectedItem, menuData.Items[i].Description);
                }

                if (menuItem != null)
                    uiMenu.AddMenuItem(menuItem);
            }

            uiMenu.OnCheckboxChange += (UIMenu menu, MenuAPI.MenuCheckboxItem menuItem, int itemIndex, bool newCheckedState) => {

                if ((bool)menuData.Items[GetIndexOfMenuItem((MenuAPI.MenuItem)menuItem)].ExecuteCallback)
                {
                    ((CheckboxItem)menuData.Items[GetIndexOfMenuItem((MenuAPI.MenuItem)menuItem)]).Checked = newCheckedState;
                    GameMode.TriggerServerEvent("MenuManager_ExecuteCallback", GetIndexOfMenuItem((MenuAPI.MenuItem)menuItem), false, JsonConvert.SerializeObject(menuData));
                }
            };

            uiMenu.OnListIndexChange += (UIMenu menu, MenuAPI.MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex) =>
            {
                var indexMenu = GetIndexOfMenuItem((MenuAPI.MenuItem)listItem);
                ((ListItem)menuData.Items[indexMenu]).SelectedItem = itemIndex;

                if (((ListItem)menuData.Items[indexMenu]).ExecuteCallbackListChange)
                {
                    GameMode.TriggerServerEvent("MenuManager_ListChanged", indexMenu, itemIndex);
                }
            };

            uiMenu.OnItemSelect += (sender, uiitem, index) =>
            {
                MenuItem menuItem = menuData.Items[GetIndexOfMenuItem(uiitem)];

                try
                {
                    if (menuItem.InputMaxLength > 0)
                    {
                        uiMenu.Visible = false;
                        /*
                        InputBox.OpenInputBox((string input) =>
                        {
                            bool valid = false;

                            if (menuItem.InputType == InputType.Number && long.TryParse(input, out long intValue))
                                valid = true;
                            else if (menuItem.InputType == InputType.UNumber && ulong.TryParse(input, out ulong uintValue))
                                valid = true;
                            else if (menuItem.InputType == InputType.Float && double.TryParse(input, out double doubleValue))
                                valid = true;
                            else if (menuItem.InputType == InputType.UFloat && double.TryParse(input, out double udoubleValue) && udoubleValue >= 0)
                                valid = true;
                            else
                                valid = true;

                            if (!valid && menuItem.InputErrorResetValue.HasValue && menuItem.InputErrorResetValue.Value)
                            {
                                menuItem.InputValue = "";
                            }
                            else if (valid)
                            {
                                menuItem.InputValue = input;

                                if (menuItem.InputSetRightLabel)
                                {
                                    menuItem.RightLabel = input;
                                    uiMenu.MenuItems[index].SetRightLabel(input);
                                }
                            }

                            uiMenu.Visible = true;

                            if (menuItem.ExecuteCallback)
                                GameMode.TriggerServerEvent("MenuManager_ExecuteCallback", index, JsonConvert.SerializeObject(menuData));
                        });*/
                    }
                    else if (menuItem.ExecuteCallback)
                        GameMode.TriggerServerEvent("MenuManager_ExecuteCallback", index, false, JsonConvert.SerializeObject(menuData));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            };


            uiMenu.OnIndexChange += (MenuAPI.Menu menu, MenuAPI.MenuItem oldItem, MenuAPI.MenuItem newItem, int oldIndex, int newIndex) =>
            {
                if (menuData.CallbackOnIndexChange)
                    GameMode.TriggerServerEvent("MenuManager_IndexChanged", newIndex);
            };

            listItem = menuData.Items;

            //uiMenu.MouseControlsEnabled = false;
            //uiMenu.MouseEdgeEnabled = false;
            //uiMenu.ControlDisablingEnabled = false;
            MenuController.AddMenu(uiMenu);
            uiMenu.OpenMenu();
        }
        #endregion

        #region Methods
        private static bool CanClose()
        {
            if (menuData == null)
                return true;
            else
                return !menuData.NoExit;
        }

        public static void CloseMenu()
        {
            CloseMenu(null);
            GameMode.TriggerServerEvent("MenuManager_ClosedMenu");
        }

        private static void CloseMenu(object[] args = null)
        {
            MenuController.CloseAllMenus();
            uiMenu = null;
        }

        public static int GetIndexOfMenuItem(MenuAPI.MenuItem item)
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                if (listItem[i].Text == item.Text)
                    return i;
            }

            return -1;
        }

        /*
        public PointF ConvertAnchorPos(float x, float y, MenuAnchor anchor, float xOffset, float yOffset)
        {
            var res = UIMenu.GetScreenResolutionMaintainRatio();
            //var res = getScreenResolution();
            switch (anchor)
            {
                case MenuAnchor.TopLeft:
                    return new PointF(x, y);
                case MenuAnchor.TopCenter:
                    return new PointF(res.Width / 2 + x, 0 + y);
                case MenuAnchor.TopRight:
                    return new PointF(res.Width - x - xOffset, y);
                case MenuAnchor.MiddleLeft:
                    return new PointF(x, res.Height / 2 + y - yOffset / 2);
                case MenuAnchor.MiddleCenter:
                    return new PointF(res.Width / 2 + x, res.Height / 2 + y - yOffset / 2);
                case MenuAnchor.MiddleRight:
                    return new PointF(res.Width - x - xOffset - 40, res.Height / 2 + y - yOffset / 2);
                case MenuAnchor.BottomLeft:
                    return new PointF(x, res.Height - y - yOffset);
                case MenuAnchor.BottomCenter:
                    return new PointF(res.Width / 2 + x, res.Height - y - yOffset);
                case MenuAnchor.BottomRight:
                    return new PointF(res.Width - x, res.Height - y - yOffset);
                default:
                    return PointF.Empty;
            }
        }*/

        public static bool HasOpen() => uiMenu != null;
        #endregion
    }
}