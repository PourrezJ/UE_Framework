using CitizenFX.Core;

namespace UE_Server
{
    public static class MenuExtension
    {
        public static bool OpenMenu(this UE_Shared.MenuManager.Menu menu, Player player)
        {
           return MenuManager.OpenMenu(player, menu);
        }
    }
}
