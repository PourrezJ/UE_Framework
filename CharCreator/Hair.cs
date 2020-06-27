using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;

namespace CharCreator
{
    internal class Hair
    {
        private static MenuListItem hairStyles;
        private static MenuListItem hairColors;
        private static MenuListItem hairHighlightColors;

        public static void AddHairsItem(Menu menu)
        {
            var pedID = Game.PlayerPed.Handle;

            List<string> overlayColorsList = new List<string>();
            for (int i = 0; i < API.GetNumHairColors(); i++)
            {
                overlayColorsList.Add($"Couleur {i + 1}");
            }

            int maxHairStyles = GetNumberOfPedDrawableVariations(pedID, 2);
            List<string> hairStylesList = new List<string>();
            for (int i = 0; i < maxHairStyles; i++)
            {
                if (API.IsPedComponentVariationValid(pedID, i, 0, 0))
                    hairStylesList.Add($"Style {i + 1}");
            }

            hairStyles = new MenuListItem("Coupe de cheveux:", hairStylesList, CharCreator.CharData.HeadOverlay[2].Index, "Sélectionnez une coiffure.");
            hairColors = new MenuListItem("Couleur de cheveux:", overlayColorsList, CharCreator.CharData.HeadOverlay[2].Color, "Sélectionnez une couleur de cheveux.") { ShowColorPanel = true, ColorPanelColorType = MenuListItem.ColorPanelType.Hair };
            hairHighlightColors = new MenuListItem("Couleur de mêche de cheveux:", overlayColorsList, CharCreator.CharData.HeadOverlay[2].SecondaryColor, "Sélectionnez une couleur de mêche des cheveux.") { ShowColorPanel = true, ColorPanelColorType = MenuListItem.ColorPanelType.Hair };

            menu.AddMenuItem(hairStyles);
            menu.AddMenuItem(hairColors);
            menu.AddMenuItem(hairHighlightColors);

            menu.OnListIndexChange += OnListIndexChange;
        }

        private static void OnListIndexChange(Menu menu, MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex)
        {
            var pedID = Game.PlayerPed.Handle;
            if (listItem == hairStyles) // hair style
            {
                CharCreator.CharData.HeadOverlay[2].Index = newSelectionIndex;
                CharCreator.ApplyChange();
            }
            else if (listItem == hairColors || listItem == hairHighlightColors) // hair colors
            {
                var tmp = (MenuListItem)menu.GetMenuItems()[1];
                int hairColor = tmp.ListIndex;
                tmp = (MenuListItem)menu.GetMenuItems()[2];
                int hairHighlightColor = tmp.ListIndex;

                CharCreator.CharData.HeadOverlay[2].Color = hairColor;
                CharCreator.CharData.HeadOverlay[2].SecondaryColor = hairHighlightColor;
                
                CharCreator.ApplyChange();
            }
        }
    }
}
