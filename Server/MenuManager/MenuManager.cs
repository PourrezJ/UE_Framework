using CitizenFX.Core;
using Newtonsoft.Json;
using UE_Server;
using UE_Shared.MenuManager;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace UE_Server
{
    public static class MenuManager
    {
        #region Private static properties
        private static ConcurrentDictionary<Player, Menu> _clientMenus = new ConcurrentDictionary<Player, Menu>();
        #endregion

        #region Constructor
        public static void Init()
        {
            GameMode.RegisterEventHandler("MenuManager_ExecuteCallback", new Action<Player, int, bool, string>(MenuManager_ExecuteCallback));
            GameMode.RegisterEventHandler("MenuManager_IndexChanged", new Action<Player, int>(MenuManager_IndexChanged));
            GameMode.RegisterEventHandler("MenuManager_ListChanged", new Action<Player, int, int>(MenuManager_ListChanged));
            GameMode.RegisterEventHandler("MenuManager_BackKey", new Action<Player>(MenuManager_BackKey));
            GameMode.RegisterEventHandler("MenuManager_ClosedMenu", new Action<Player>(MenuManager_ClosedMenu));
        }

        #endregion

        #region API Event handlers
        public static void OnPlayerDisconnect(Player player)
        {
            _clientMenus.TryRemove(player, out _);
        }
        #endregion

        #region Private API triggers
        public static void MenuManager_BackKey([FromSource]Player player)
        {
            _clientMenus.TryGetValue(player, out Menu menu);

            if (menu != null && !menu.BackCloseMenu)
            {
                if (menu.ItemSelectCallbackAsync != null)
                    Task.Run(async () => { await menu.ItemSelectCallbackAsync(player, menu, null, -1); });

                menu.ItemSelectCallback?.Invoke(player, menu, null, -1);
            }
            else if (menu != null)
            {
                if (menu.FinalizerAsync != null)
                    Task.Run(async () => { await menu.FinalizerAsync(player, menu); });

                menu.Finalizer?.Invoke(player, menu);
                _clientMenus.TryRemove(player, out _);
            }
        }

        private static void MenuManager_ExecuteCallback([FromSource]Player player, int itemIndex, bool forced, string datastr)
        {
            _clientMenus.TryGetValue(player, out Menu menu);

            if (menu != null)
            {
                dynamic data = JsonConvert.DeserializeObject(datastr);

                foreach (MenuItem menuItem in menu.Items)
                {
                    try
                    {
                        if (menuItem.Type == MenuItemType.CheckboxItem)
                            ((CheckboxItem)menuItem).Checked = data[menuItem.Id];
                        else if (menuItem.Type == MenuItemType.ListItem)
                            ((ListItem)menuItem).SelectedItem = data[menuItem.Id]["Index"];
                        else if (menuItem.InputMaxLength > 0)
                            menuItem.InputValue = data[menuItem.Id];
                    }
                    catch (Exception)
                    { }
                }

                try
                {
                    if (itemIndex >= menu.Items.Count)
                        return;

                    MenuItem menuItem = menu.Items[itemIndex];

                    if (menuItem == null)
                        return;

                    if (menu.ItemSelectCallbackAsync != null)
                        Task.Run(async () => { await menu.ItemSelectCallbackAsync(player, menu, menuItem, itemIndex); });

                    menu.ItemSelectCallback?.Invoke(player, menu, menuItem, itemIndex);

                    if (menuItem.OnMenuItemCallbackAsync != null)
                        Task.Run(async () => { await menuItem.OnMenuItemCallbackAsync(player, menu, menuItem, itemIndex); });

                    menuItem.OnMenuItemCallback?.Invoke(player, menu, menuItem, itemIndex);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        public static void MenuManager_IndexChanged([FromSource]Player player, int index)
        {
            _clientMenus.TryGetValue(player, out Menu menu);

            if (menu != null)
            {
                if (menu.IndexChangeCallbackAsync != null)
                    Task.Run(async () => { await menu.IndexChangeCallbackAsync(player, menu, index, menu.Items[index]); });

                menu.IndexChangeCallback?.Invoke(player, menu, index, menu.Items[index]);
            }
        }

        public static void MenuManager_ListChanged([FromSource]Player player, int itemID, int itemValue)
        {
            _clientMenus.TryGetValue(player, out Menu menu);

            if (menu != null)
            {
                if (menu.ListItemChangeCallbackAsync != null)
                    Task.Run(async () => { await menu.ListItemChangeCallbackAsync(player, menu, (ListItem)menu.Items[itemID], itemValue); });

                menu.ListItemChangeCallback?.Invoke(player, menu, (ListItem)menu.Items[itemID], itemValue);
            }
        }

        public static void MenuManager_ClosedMenu([FromSource]Player player)
        {
            lock (_clientMenus)
            {
                _clientMenus.TryGetValue(player, out Menu menu);

                if (menu != null)
                {
                    if (menu.FinalizerAsync != null)
                        Task.Run(async () => await menu.FinalizerAsync(player, menu));

                    menu.Finalizer?.Invoke(player, menu);
                    _clientMenus.TryRemove(player, out _);
                }
            }
        }
        #endregion

        #region Public static methods
        public static void CloseMenu(Player client)
        {
            lock (_clientMenus)
            {
                if (_clientMenus.TryRemove(client, out Menu menu) && menu != null)
                {
                    if (menu.FinalizerAsync != null)
                        Task.Run(async () => { await menu.FinalizerAsync(client, menu); });

                    menu.Finalizer?.Invoke(client, menu);
                }

                client.TriggerEvent("MenuManager_CloseMenu");
            }
        }

        public static void ForceCallback(Player client)
        {
            _clientMenus.TryGetValue(client, out Menu menu);

            if (menu != null && (menu.ItemSelectCallbackAsync != null || menu.ItemSelectCallback != null))
                client.TriggerEvent("MenuManager_ForceCallback");
        }

        public static Menu GetMenu(Player client)
        {
            _clientMenus.TryGetValue(client, out Menu menu);
            return menu;
        }

        public static bool HasOpenMenu(Player client)
        {
            return _clientMenus.ContainsKey(client);
        }

        public static bool OpenMenu(Player client, Menu menu)
        {
            if (menu.Items.Count == 0 || menu.Items == null)
                return false;

            lock (_clientMenus)
            {
                _clientMenus.TryRemove(client, out Menu oldMenu);

                if (oldMenu != null)
                {
                    if (oldMenu.FinalizerAsync != null)
                        Task.Run(async () => { await oldMenu.FinalizerAsync(client, menu); });

                    oldMenu.Finalizer?.Invoke(client, menu);
                }

                if (_clientMenus.TryAdd(client, menu))
                {
                    string json = JsonConvert.SerializeObject(menu, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    Logger.Info(json);
                    client.TriggerEvent("MenuManager_OpenMenu", json);
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}