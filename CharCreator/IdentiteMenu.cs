using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE_Client_Extented;
using UE_Shared;

namespace CharCreator
{
    internal class IdentiteMenu
    {
        private static MenuItem lastName = new MenuItem("Nom:");
        private static MenuItem firstName = new MenuItem("Prénom:");
        private static MenuItem age = new MenuItem("Âge:");
        private static MenuItem nationality = new MenuItem("Nationalité:");
        private static MenuListItem sex = new MenuListItem("Sexe", new List<string>() { "Homme", "Femme" }, 0, "Choissez le sexe de votre personnage.");


        public static void CreateMenu(Menu ParentMenu)
        {
            Menu identite = new Menu("Identité");
            identite.OnItemSelect += OnItemSelect;
            identite.OnListIndexChange += OnListIndexChange;

            identite.AddMenuItem(lastName);
            identite.AddMenuItem(firstName);
            identite.AddMenuItem(age);
            identite.AddMenuItem(nationality);

            identite.AddMenuItem(sex);

            MenuItem bindmenu = new MenuItem("Identité");
            bindmenu.Description =
                $"Nom: {CharCreator.CharData.Identite.LastName} \n" +
                $"Prénom: {CharCreator.CharData.Identite.FirstName} \n" +
                $"Âge: {CharCreator.CharData.Identite.Age} \n" +
                $"Nationalité: {CharCreator.CharData.Identite.Nationality}";

            UpdateNameMenu();

            ParentMenu.AddMenuItem(bindmenu);
            MenuController.BindMenuItem(ParentMenu, identite, bindmenu);
        }

        public static void UpdateNameMenu()
        {
            lastName.Label = CharCreator.CharData.Identite.LastName;
            firstName.Label = CharCreator.CharData.Identite.FirstName;
            age.Label = CharCreator.CharData.Identite.Age.ToString();
            nationality.Label = CharCreator.CharData.Identite.Nationality;

            switch(CharCreator.CharData.Identite.Gender)
            {
                case Sex.Male:
                    sex.Label = "Homme";
                    break;

                case Sex.Girl:
                    sex.Label = "Femme";
                    break;

                case Sex.Other:
                    sex.Label = "Non-binaire";
                    break;
            }
        }

        private static async void OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            switch (menuItem.Text)
            {
                case "Nom:":
                    CharCreator.CharData.Identite.LastName = await Inputbox.GetUserInput(CharCreator.CharData.Identite.LastName);
                    break;

                case "Prénom:":
                    CharCreator.CharData.Identite.FirstName = await Inputbox.GetUserInput(CharCreator.CharData.Identite.FirstName);
                    break;

                case "Âge:":

                    var age = Convert.ToInt32(await Inputbox.GetUserInput(CharCreator.CharData.Identite.Age.ToString()));

                    if (!Config.Get<bool>("canUnder18") && age < 18)
                    {
                        // Need Notification!
                        return;
                    }

                    CharCreator.CharData.Identite.Age = age;
                    break;

                case "Nationalité:":
                    CharCreator.CharData.Identite.Nationality = await Inputbox.GetUserInput(CharCreator.CharData.Identite.Nationality);
                    break;
            }

            UpdateNameMenu();
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
            CharCreator.ApplyChange();
        }
    }
}
