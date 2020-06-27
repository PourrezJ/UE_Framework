using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace CharCreator
{
    internal class Beard
    {
        private static MenuListItem beardStyles;
        private static MenuListItem beardOpacity;
        private static MenuListItem beardColor;

        public static void AddBeardsItem(Menu menu)
        {
            List<string> overlayColorsList = new List<string>();
            for (int i = 0; i < GetNumHairColors(); i++)
                overlayColorsList.Add($"Couleur #{i + 1}");

            List<string> beardStylesList = new List<string>();
            for (int i = 0; i < GetNumHeadOverlayValues(1); i++)
                beardStylesList.Add($"Style #{i + 1}");
            
            beardStyles = new MenuListItem("Style de barbe:", beardStylesList, CharCreator.CharData.HeadOverlay[1].Index, "Sélectionnez une coiffure barbe / faciale.");
            beardOpacity = new MenuListItem("Opacité de la barbe:", Appearance.OpacityList, (int)CharCreator.CharData.HeadOverlay[1].Opacity * 10, "Sélectionnez l'opacité de votre barbe / poils du visage.") { ShowOpacityPanel = true };
            beardColor = new MenuListItem("Couleur de barbe:", overlayColorsList, CharCreator.CharData.HeadOverlay[1].Color, "Sélectionnez une couleur de barbe.") { ShowColorPanel = true, ColorPanelColorType = MenuListItem.ColorPanelType.Hair };

            menu.OnListIndexChange += OnListIndexChange;

            menu.AddMenuItem(beardStyles);
            menu.AddMenuItem(beardOpacity);
            menu.AddMenuItem(beardColor);
        }

        private static void OnListIndexChange(Menu menu, MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex)
        {
            if (listItem == beardStyles)
            {
                CharCreator.CharData.HeadOverlay[1].Index = newSelectionIndex;
                CharCreator.ApplyChange();
            }
            else if (listItem == beardOpacity)
            {
                CharCreator.CharData.HeadOverlay[1].Opacity = newSelectionIndex * 0.1f;
                CharCreator.ApplyChange();
            }
            else if (listItem == beardColor)
            {
                CharCreator.CharData.HeadOverlay[1].Color = newSelectionIndex;
                CharCreator.ApplyChange();
            }
        }
    }
}
