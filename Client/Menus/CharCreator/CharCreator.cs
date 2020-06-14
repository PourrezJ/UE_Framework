using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UE_Client.Menus
{
    public enum Sex
    {
        Male,
        Girl
    }

    public enum SkinColor
    {
        White1,
        White2,
        Black1,
        Black2,
        Yellow1,
        Yellow2,
        Monster
    }

    public struct SkinModel
    {
        public SkinColor SkinColor;
        public uint HeadHash;

        public SkinModel(SkinColor skinColor, uint headHash)
        {
            SkinColor = skinColor;
            HeadHash = headHash;
        }
    }

    public struct Identite
    {
        public string LastName;
        public string FirstName;
        public int Age;
        public string Nationality;
    }

    public struct CharData
    {
        public int Carrure;
        public uint BodyStyle;
        public uint FeetStyle;
        public int Clothing;
        public SkinColor SkinColor;
        public Sex Gender;
        public uint Head;
        public uint Hair;
        public Identite Identite;
    }

    public static partial class CharCreator
    {
        private static CharData charData;
        private static Menu menu;

        internal static void Init()
        {
            GameMode.RegisterEventHandler("OpenCharCreator", new Action(OpenCharCreator));
            GameMode.RegisterTickHandler(OnTick);
        }

        internal static async void OpenCharCreator()
        {
            //var intdata = new Interior(new Vector3(-561.8157f, -3780.966f, 239.0805f), "mp_char_male_mirror");
            //int interiorID = Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, intdata.Position.X, intdata.Position.Y, intdata.Position.Z);
            /*
            if (!Function.Call<bool>(Hash.IS_INTERIOR_ENTITY_SET_ACTIVE, interiorID, intdata.Name))
            {
                Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, interiorID, intdata.Name, 0);
            }*/
            Function.Call(Hash.DRAW_LIGHT_WITH_RANGE, -561.8157f, -3780.966f, 239.0805f, 255, 255, 255, 50f, 100f);

            charData = new CharData();
            charData.Identite = new Identite();

            var cam = World.CreateCamera(new Vector3(-561.8157f, -3780.966f, 239.0805f), new Vector3(-4.2146f, -0.0007f, -87.8802f), 2);
            cam.FieldOfView = 30f;
            World.RenderingCamera = cam;

            await Game.Player.ChangeModel("mp_male");
            Game.Player.Character.IsPositionFrozen = true;
            Game.Player.Character.Position = new Vector3(-558.3412f, -3780.959f, 237.6013f);            
            Game.Player.Character.Rotation = new Vector3(0,0,95f);
            Game.Player.Character.Task.TurnTo(new Vector3(-561.8157f, -3780.966f, 239.0805f), 15000);


           // Function.Call((Hash)0xBE83CAE8ED77A94F, Game.GenerateHash("sunny"));
            World.CurrentDayTime = new TimeSpan(10, 0, 0);
            //Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "Online_Character_Editor");

            //Function.Call(Hash._CREATE_ANIM_SCENE, "lightrig@online_character_editor", 0, 0, false, true);

            Game.Player.Character.SetConfigFlag(130, true);
            Game.Player.Character.SetConfigFlag(315, true);
            Game.Player.Character.SetConfigFlag(301, true);
            

           menu = new Menu("Création", "Personnage");
            menu.OnMenuClose += (uimenu) =>
            {
                if (uimenu == menu)
                {
                    menu.Visible = true;
                }
            };

            menu.OnItemSelect += Menu_OnItemSelect;

            #region Identité
            Menu identite = new Menu("Identité");
            identite.OnItemSelect += Menu_OnItemSelect;

            identite.AddMenuItem(new MenuItem("Nom:"));
            identite.AddMenuItem(new MenuItem("Prénom:"));
            identite.AddMenuItem(new MenuItem("Âge:"));
            identite.AddMenuItem(new MenuItem("Nationalité:"));

            MenuItem bindmenu = new MenuItem("Identité");
            menu.AddMenuItem(bindmenu);
            MenuController.BindMenuItem(menu, identite, bindmenu);
            #endregion

            #region Apparance
            menu.AddMenuItem(new MenuListItem("Sexe", new List<string>() { "Homme", "Femme" }, 0, "Choissez le sexe de votre personnage."));
            menu.OnListIndexChange += OnListIndexChange;

            Menu apparance = new Menu("Apparence");
            bindmenu = new MenuItem("Apparence");
            menu.AddMenuItem(bindmenu);
            MenuController.BindMenuItem(menu, apparance, bindmenu);

            string changeCallback(MenuDynamicListItem item, bool left)
            {
                int a = (left) ? (int.Parse(item.CurrentItem) - 1) : (int.Parse(item.CurrentItem) + 1);

                if (a < 0) a = 0;

                if (item.Text == "Apparence de base")
                {
                    var headList = (charData.Gender == Sex.Male) ? MaleHeads : FemaleHeads;

                    if (a > headList.Length)
                        a = headList.Length;

                    charData.Head = headList[a].HeadHash;

                    if (charData.SkinColor != headList[a].SkinColor)
                    {
                        charData.SkinColor = headList[a].SkinColor;
                        SkinColorChange(headList[a].SkinColor);
                    }             
                }
                else if (item.Text == "Coupe de cheveux")
                {
                    var hairList = (charData.Gender == Sex.Male) ? MaleHairs : FemaleHairs;
                    if (a > hairList.Length)
                        a = hairList.Length;

                    charData.Hair = hairList[a];
                }

                ApplyChange();
                return a.ToString();
            }
            MenuDynamicListItem dynList = new MenuDynamicListItem("Apparence de base", "0", new MenuDynamicListItem.ChangeItemCallback(changeCallback), "Modifiez l'apparence de base de votre personnage.");
            apparance.AddMenuItem(dynList);

            apparance.AddMenuItem(new MenuListItem("Carrure", new List<string>() { "Lourd", "Musclé" , "Athlétique", "Maigre", "Dans la moyenne" }, 0, "Choissez la carrure de votre personnage."));

            MenuDynamicListItem hairList = new MenuDynamicListItem("Coupe de cheveux", "0", new MenuDynamicListItem.ChangeItemCallback(changeCallback), "Modifiez la coupe de cheveux de votre personnage.");
            apparance.AddMenuItem(hairList);

            apparance.OnListIndexChange += OnListIndexChange;
            #endregion

            MenuController.AddMenu(menu);
            menu.OpenMenu();
        }

        private static async void OnListIndexChange(Menu menu, MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex)
        {
            if (listItem.Text == "Sexe")
            {
                if (newSelectionIndex == 0)
                {
                    charData.Gender = Sex.Male;
                    await Game.Player.ChangeModel("mp_male");
                }
                else
                {
                    charData.Gender = Sex.Girl;
                    await Game.Player.ChangeModel("mp_female");
                }
                Function.Call((Hash)0x283978A15512B2FE, API.PlayerPedId(), true); // default compoment
            }
            else if (listItem.Text == "Carrure")
            {
                switch (listItem.ListIndex)
                {
                    case 0:
                        charData.Carrure = Game.Player.Character.Model == "mp_male" ? 124 : 110;  
                        break;

                    case 1:
                        charData.Carrure = Game.Player.Character.Model == "mp_male" ? 125 : 111;
                        break;

                    case 2:
                        charData.Carrure = Game.Player.Character.Model == "mp_male" ? 126 : 112;
                        break;

                    case 3:
                        charData.Carrure = Game.Player.Character.Model == "mp_male" ? 127 : 113;
                        break;

                    case 4:
                        charData.Carrure = Game.Player.Character.Model == "mp_male" ? 128 : 114;
                        break;
                }
                ApplyChange();
            }
        }

        private static async void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            if (menu.MenuTitle == "Création")
            {

            }
            else if (menu.MenuTitle == "Identité")
            {
                switch (menuItem.Text)
                {
                    case "Nom:":
                        charData.Identite.LastName = await Inputbox.GetUserInput("");
                        menuItem.Label = charData.Identite.LastName;
                        break;

                    case "Prénom:":
                        charData.Identite.FirstName = await Inputbox.GetUserInput("");
                        menuItem.Label = charData.Identite.FirstName;
                        break;

                    case "Âge:":
                        charData.Identite.Age = Convert.ToInt32(await Inputbox.GetUserInput(""));
                        menuItem.Label = charData.Identite.Age.ToString();
                        break;

                    case "Nationalité:":
                        charData.Identite.Nationality = await Inputbox.GetUserInput("");
                        menuItem.Label = charData.Identite.Nationality;
                        break;
                }
            }
        }

        private static void ApplyChange()
        {
            /*
            Function.Call((Hash)0x283978A15512B2FE, API.PlayerPedId(), true); // reset
            Function.Call((Hash)0xA5BAE410B03E7371, API.PlayerPedId(), charData.Carrure, 0, 0, 0);
            Function.Call((Hash)0xD3A7B003ED343FD9, Game.Player.Character.Handle, charData.BodyStyle, true, true, false);
            Function.Call((Hash)0xD3A7B003ED343FD9, Game.Player.Character.Handle, charData.FeetStyle, true, true, false);
            Function.Call((Hash)0xD3A7B003ED343FD9, Game.Player.Character.Handle, charData.Head, true, true, false);
            Function.Call((Hash)0xD3A7B003ED343FD9, Game.Player.Character.Handle, charData.Hair, true, true, false);
            */
        }

        private static Task OnTick()
        {
            if (menu != null)
            {
                Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);
            }

            return Task.FromResult(0);
        }

        private static void SkinColorChange(SkinColor skinColor)
        {
            /*
            switch (skinColor)
            {
                case SkinColor.Black1:
                    charData.BodyStyle = (uint)((charData.Gender == Sex.Male) ? 0X05C1686B : 0X05C1686B);
                    charData.FeetStyle = (uint)((charData.Gender == Sex.Male) ? 0X27F95FB6 : 0X05C1686B);
                    // Men
                    // Coats 44
                    // Pant 212
                    // Boots 49
                    break;

                case SkinColor.Black2:
                    charData.BodyStyle = 0X5EA984F8;
                    charData.FeetStyle = 0x35D1FB67;
                    break;

                case SkinColor.White1:
                    charData.BodyStyle = 0X4BD8F4A1;
                    charData.FeetStyle = 0x84BAA309;
                    break;

                case SkinColor.White2:
                    charData.BodyStyle = 0X5A929214;
                    charData.FeetStyle = 0XEA27EDE2;
                    break;

                case SkinColor.Yellow1:
                    charData.BodyStyle = 0X62FA3A88;
                    charData.FeetStyle = 0XF2EA7BDE;
                    break;

                case SkinColor.Yellow2:
                    charData.BodyStyle = 0X5A929214;
                    charData.FeetStyle = 0XEA27EDE2;
                    break;
            }*/
        }
    }
}
