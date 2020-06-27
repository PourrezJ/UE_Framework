using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE_Shared;

namespace CharCreator
{
    internal class HerritanceMenu
    {
        public static HerritageMenu HerritageMenu { get; private set; }

        public static void CreateMenu(Menu Parent)
        {
            HerritageMenu = new HerritageMenu("Héritage");
            HerritageMenu.OnSliderPositionChange += OnSliderPositionChange;
            HerritageMenu.OnListIndexChange += OnListIndexChange;
            HerritageMenu.AddMenuItem(new MenuListItem("Père", ((Dad[])Enum.GetValues(typeof(Dad))).Select(c => c.ToString()).ToList(), 0, "Choissez le père de votre personnage."));
            HerritageMenu.AddMenuItem(new MenuListItem("Mère", ((Mum[])Enum.GetValues(typeof(Mum))).Select(c => c.ToString()).ToList(), 0, "Choissez la mère de votre personnage."));

            var slider = new MenuSliderItem("Ressemblance", 0, 10, 5);
            slider.SliderLeftIcon = MenuItem.Icon.FEMALE;
            slider.SliderRightIcon = MenuItem.Icon.MALE;
            HerritageMenu.AddMenuItem(slider);

            slider = new MenuSliderItem("Couleur de peau", 0, 10, 5);
            slider.SliderLeftIcon = MenuItem.Icon.FEMALE;
            slider.SliderRightIcon = MenuItem.Icon.MALE;
            HerritageMenu.AddMenuItem(slider);

            var bindmenu = new MenuItem("Héritage");
            Parent.AddMenuItem(bindmenu);

            MenuController.BindMenuItem(Parent, HerritageMenu, bindmenu);
        }

        private static async void OnListIndexChange(Menu menu, MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex)
        {
            if (listItem.Text == "Sexe")
            {
                if (newSelectionIndex == 0)
                {
                    CharCreator.CharData.Identite.Gender = Sex.Male;
                    await Game.Player.ChangeModel(PedHash.FreemodeMale01);
                }
                else
                {
                    CharCreator.CharData.Identite.Gender = Sex.Girl;
                    await Game.Player.ChangeModel(PedHash.FreemodeFemale01);
                }
                API.SetPedDefaultComponentVariation(Game.PlayerPed.Handle);
            }
            else if (listItem.Text == "Mère")
            {
                (menu as HerritageMenu).CurrentMum = ((Mum)newSelectionIndex);
                CharCreator.CharData.HeadBlendData.ShapeFirst = (byte)newSelectionIndex;
            }
            else if (listItem.Text == "Père")
            {
                (menu as HerritageMenu).CurrentDad = ((Dad)newSelectionIndex);
                CharCreator.CharData.HeadBlendData.ShapeSecond = (byte)newSelectionIndex;
            }
            CharCreator.ApplyChange();
        }

        private static void OnSliderPositionChange(Menu menu, MenuSliderItem sliderItem, int oldPosition, int newPosition, int itemIndex)
        {
            Debug.WriteLine((newPosition * 0.1f).ToString());
            if (sliderItem.Text == "Ressemblance")
            {
                //ShapeMixItem 
                CharCreator.CharData.HeadBlendData.ShapeMix = newPosition * 0.1f;
            }
            else if (sliderItem.Text == "Couleur de peau")
            {
                //SkinMixItem
                CharCreator.CharData.HeadBlendData.SkinMix = newPosition * 0.1f;
            }
            CharCreator.ApplyChange();
        }
    }
}
