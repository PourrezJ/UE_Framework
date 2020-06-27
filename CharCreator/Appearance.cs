using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace CharCreator
{
    internal class Appearance
    {
        public static List<string> OpacityList = new List<string>() { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };


        public static void CreateMenu(Menu ParentMenu)
        {
            Menu menu = new Menu("Apparence");


            Hair.AddHairsItem(menu);
            Beard.AddBeardsItem(menu);



            MenuItem bindmenu = new MenuItem("Apparence");
            ParentMenu.AddMenuItem(bindmenu);
            MenuController.BindMenuItem(ParentMenu, menu, bindmenu);
        }

        private static void OnListIndexChange(Menu menu, MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex)
        {         
            if (itemIndex == 33) // eye color
            {
                int selection = ((MenuListItem)menu.GetMenuItems()[itemIndex]).ListIndex;
                SetPedEyeColor(Game.PlayerPed.Handle, selection);
                //Functions.CurrentCharacter.PedAppearance.EyeColor = selection;
            }
            else
            {
                int selection = ((MenuListItem)menu.GetMenuItems()[itemIndex]).ListIndex;
                float opacity = 0f;
                if (menu.GetMenuItems()[itemIndex + 1] is MenuListItem)
                    opacity = (((float)((MenuListItem)menu.GetMenuItems()[itemIndex + 1]).ListIndex + 1) / 10f) - 0.1f;
                else if (menu.GetMenuItems()[itemIndex - 1] is MenuListItem)
                    opacity = (((float)((MenuListItem)menu.GetMenuItems()[itemIndex - 1]).ListIndex + 1) / 10f) - 0.1f;
                else if (menu.GetMenuItems()[itemIndex] is MenuListItem)
                    opacity = (((float)((MenuListItem)menu.GetMenuItems()[itemIndex]).ListIndex + 1) / 10f) - 0.1f;
                else
                    opacity = 1f;
                switch (itemIndex)
                {
                    case 3: // blemishes
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 0, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.BlemishesStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.BlemishesOpacity = opacity;
                        break;
                    case 5: // beards
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 1, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.BeardStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.BeardOpacity = opacity;
                        break;
                    case 7: // beards color
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, selection, selection);
                        //Functions.CurrentCharacter.PedAppearance.BeardColor = selection;
                        break;
                    case 8: // eyebrows
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 2, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.EyebrowsStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.EyebrowsOpacity = opacity;
                        break;
                    case 10: // eyebrows color
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 2, 1, selection, selection);
                        //Functions.CurrentCharacter.PedAppearance.EyebrowsColor = selection;
                        break;
                    case 11: // ageing
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 3, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.AgeingStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.AgeingOpacity = opacity;
                        break;
                    case 13: // makeup
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 4, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.MakeupStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.MakeupOpacity = opacity;
                        break;
                    case 15: // makeup color
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 4, 2, selection, selection);
                        //Functions.CurrentCharacter.PedAppearance.MakeupColor = selection;
                        break;
                    case 16: // blush style
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 5, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.BlushStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.BlushOpacity = opacity;
                        break;
                    case 18: // blush color
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 5, 2, selection, selection);
                        //Functions.CurrentCharacter.PedAppearance.BlushColor = selection;
                        break;
                    case 19: // complexion
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 6, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.ComplexionStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.ComplexionOpacity = opacity;
                        break;
                    case 21: // sun damage
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 7, selection, opacity);
                       // Functions.CurrentCharacter.PedAppearance.SunDamageStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.SunDamageOpacity = opacity;
                        break;
                    case 23: // lipstick
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 8, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.LipstickStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.LipstickOpacity = opacity;
                        break;
                    case 25: // lipstick color
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 8, 2, selection, selection);
                        //Functions.CurrentCharacter.PedAppearance.LipstickColor = selection;
                        break;
                    case 26: // moles and freckles
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 9, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.MolesFrecklesStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.MolesFrecklesOpacity = opacity;
                        break;
                    case 28: // chest hair
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 10, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.ChestHairStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.ChestHairOpacity = opacity;
                        break;
                    case 30: // chest hair color
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 10, 1, selection, selection);
                        //Functions.CurrentCharacter.PedAppearance.ChestHairColor = selection;
                        break;
                    case 31: // body blemishes
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 11, selection, opacity);
                        //Functions.CurrentCharacter.PedAppearance.BodyBlemishesStyle = selection;
                        //Functions.CurrentCharacter.PedAppearance.BodyBlemishesOpacity = opacity;
                        break;
                }
            }
        }
    }
}
