using CitizenFX.Core;
using UE_Shared.MenuManager;

namespace UE_Server.Entities
{
    public static class PlayerMenu
    {
        public static void OpenMenu(Player player)
        {
            Menu menu = new Menu("", "Menu Joueur", "test");

            menu.Add(new MenuItem("ID_WalkStyle", "Style de marche"));
            menu.Add(new MenuItem("ID_Staff", "Staff"));
            
            menu.OpenMenu(player);
        }
    }
}
