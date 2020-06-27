using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UE_Client_Extented;
using UE_Shared;
using Utils = UE_Client_Extented.Utils;

namespace CharCreator
{
    public class CharCreator : BaseScript
    {
        #region Fields
        private bool devDebug;

        internal static PedCharacter CharData;
        internal static Menu ParentMenu;
        internal static Camera Camera;

        private static MenuItem confirmationItem = new MenuItem("~r~ Confirmer");
        #endregion

        #region CharCreator
        public CharCreator()
        {
            //this.EventHandlers["OpenCharCreator"] += new Action(OpenCharCreator);
            Config.LoadConfig();
        }
        #endregion

        #region Methods
        internal static async void OpenCharCreator()
        {
            CharData = new PedCharacter();

            await Utils.PerformRequest(PedHash.FreemodeMale01);
            await Utils.PerformRequest(PedHash.FreemodeFemale01);
            await Game.Player.ChangeModel(PedHash.FreemodeMale01);
            ApplyChange();

            var pos = JsonConvert.DeserializeObject<Vector3>(Config.Get<string>("pos"));
            Game.Player.Character.PositionNoOffset = pos;
            Game.Player.Character.Heading = Config.Get<float>("heading");

            World.CurrentDayTime = new TimeSpan(12, 0, 0);
            Game.PauseClock(true);

            Camera = new Camera(API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true));
            Camera.IsActive = true;
            API.RenderScriptCams(true, false, 0, false, false);
            Camera.Position = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 0.5f + new Vector3(0, 0, 0.8f);
            Camera.PointAt(Game.PlayerPed.Position + new Vector3(0, 0, 0.8f));

            API.ShutdownLoadingScreen();
            API.ShutdownLoadingScreenNui();

            Screen.Hud.IsRadarVisible = false;
            Screen.Hud.IsVisible = false;
            Screen.Fading.FadeIn(0);

            ParentMenu = new Menu("Création", "Personnage");
            ParentMenu.OnMenuClose += OnMenuClose;

            IdentiteMenu.CreateMenu(ParentMenu);
            HerritanceMenu.CreateMenu(ParentMenu);
            Appearance.CreateMenu(ParentMenu);

            ParentMenu.AddMenuItem(confirmationItem);

            ParentMenu.HeaderTexture = new KeyValuePair<string, string>();
            ParentMenu.OnItemSelect += OnItemSelect;

            MenuController.AddMenu(ParentMenu);
            ParentMenu.OpenMenu();
        }

        private static void OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            if (menuItem == confirmationItem)
            {
                TriggerServerEvent("Charcreator_End", JsonConvert.SerializeObject(CharData));
                ParentMenu.CloseMenu();
                ParentMenu = null;
            }
        }

        internal static async void ApplyChange()
        {
            var ped = Game.PlayerPed.Handle;

            if (Game.PlayerPed.Model != ((CharData.Identite.Gender == Sex.Male) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01))
                await Game.Player.ChangeModel(PedHash.FreemodeMale01);

            API.SetPedDefaultComponentVariation(ped);
            API.ClearPedFacialDecorations(ped);

            var hbd = CharData.HeadBlendData;
            API.SetPedHeadBlendData(ped, hbd.ShapeFirst, hbd.ShapeSecond, 0, hbd.ShapeFirst, hbd.ShapeSecond, 0, hbd.ShapeMix, hbd.SkinMix, 0, false);
            API.SetPedComponentVariation(ped, 2, CharData.HeadOverlay[2].Index, 0, 0);
            API.SetPedHairColor(ped, CharData.HeadOverlay[2].Color, CharData.HeadOverlay[2].SecondaryColor);
            API.SetPedHeadOverlay(ped, 1, CharData.HeadOverlay[1].Index, CharData.HeadOverlay[1].Opacity);
            API.SetPedHeadOverlayColor(ped, 1, 1, CharData.HeadOverlay[1].Color, CharData.HeadOverlay[1].Color);
        }
        #endregion

        #region MenuAPI Events
        private static void OnMenuClose(Menu menu)
        {
            if (menu == ParentMenu)
            {
                ParentMenu.Visible = true;
            }
        }
        #endregion

        #region Resource Events
        [Tick]
        public Task OnTick()
        {
            if (ParentMenu != null)
            {
                Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);
            }
            if (!devDebug)
            {
                devDebug = true;
                OpenCharCreator();    
            }

            return Task.FromResult(0);
        }
        #endregion
    }
}
