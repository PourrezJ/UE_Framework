using CitizenFX.Core;
using CitizenFX.Core.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using UE_Shared;
using static CitizenFX.Core.Native.API;

namespace CharCreator
{
    public class Main : BaseScript
    {
        private static bool CharOpenned;
        private static Camera Camera;

        public Main()
        {
            Debug.WriteLine("Starting CharCreator module.");

            Config.LoadConfig("Config.ini");

            EventHandlers["OpenCharCreator"] += new Action(OpenCharCreator);
            EventHandlers["onClientResourceStop"] += new Action<string>(OnClientResourceStop);
        }

        private void OnClientResourceStop(string obj)
        {
            Camera?.Delete();
        }

        public async void OpenCharCreator()
        {
            Debug.WriteLine("Open CharCreator.");

            await UE_Client_Extented.Utils.PerformRequest(CitizenFX.Core.PedHash.FreemodeMale01);
            await UE_Client_Extented.Utils.PerformRequest(CitizenFX.Core.PedHash.FreemodeFemale01);

            await Game.Player.ChangeModel(CitizenFX.Core.PedHash.FreemodeMale01);
            SetPedDefaultComponentVariation(Game.PlayerPed.Handle);

            var pos = JsonConvert.DeserializeObject<Vector3>(Config.Get<string>("pos"));
            Game.Player.Character.PositionNoOffset = pos;
            Game.Player.Character.Heading = Config.Get<float>("heading");

            // Camera
            /*
            Vector3 CamPos = (pos + new Vector3(0,0,0.8f)).Forward(0, 0.8f);

            Camera = new UE_Client_Extented.Camera(CamPos, new Vector3(0,0,180));
            Camera.SetActiveCamera(true);
            */

            Camera = new Camera(CreateCam("DEFAULT_SCRIPTED_CAMERA", true));
            Camera.IsActive = true;
            RenderScriptCams(true, false, 0, false, false);
            Camera.Position = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 0.5f + new Vector3(0, 0, 0.8f);
            // Camera.PointAt(Game.PlayerPed.Bones[Bone.FACIAL_facialRoot]);
            Camera.PointAt(Game.PlayerPed.Position + new Vector3(0,0,0.8f));

            //

            Screen.Hud.IsRadarVisible = false;

            FreezeEntityPosition(Game.Player.Character.Handle, true);
            ClearPedTasksImmediately(Game.PlayerPed.Handle);


            ShutdownLoadingScreenNui();
            ShutdownLoadingScreen();

            DoScreenFadeIn(0);

            SetNuiFocus(true, true);

            RegisterNUICallback("updateSkin", UpdateSkin);
            RegisterNUICallback("rotateleftheading", RotateLeftHeading);
            RegisterNUICallback("rotaterightheading", RotateRightHeading);
            RegisterNUICallback("zoom", Zoom);

            SendNuiMessage(JsonConvert.SerializeObject(new { openSkinCreator = true }));

            CharOpenned = true;
        }

        private CallbackDelegate Zoom(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (!CharOpenned)
                return result;

            return result;
        }

        private CallbackDelegate RotateRightHeading(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (!CharOpenned)
                return result;

            Game.PlayerPed.Heading += 1f;
            
            return result;
        }

        private CallbackDelegate RotateLeftHeading(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (!CharOpenned)
                return result;

            Game.PlayerPed.Heading -= 1f;

            return result;
        }

        private CallbackDelegate UpdateSkin(IDictionary<string, dynamic> data, CallbackDelegate result)
        {
            if (!CharOpenned)
                return result;

            var ped = Game.Player.Character.Handle;

            Debug.WriteLine(JsonConvert.SerializeObject(data));

            var playerChar = new PedCharacter()
            {
                Dad = Convert.ToInt32(data["dad"]),
                Mum = Convert.ToInt32(data["mum"]),
                DadMumPercent = Convert.ToInt32(data["dadmumpercent"]),
                Skin = Convert.ToInt32(data["skin"]),
                EyeColor = Convert.ToInt32(data["eyecolor"]),
                Acne = Convert.ToInt32(data["acne"]),
                SkinProblem = Convert.ToInt32(data["skinproblem"]),
                Freckle = Convert.ToInt32(data["freckle"]),
                Wringle = Convert.ToInt32(data["wrinkle"]),
                WringleOpacity = Convert.ToInt32(data["wrinkleopacity"]),
                Hair = Convert.ToInt32(data["hair"]),
                HairColor = Convert.ToInt32(data["haircolor"]),
                EyeBrow = Convert.ToInt32(data["eyebrow"]),
                EyeBrowOpacity = Convert.ToInt32(data["eyebrowopacity"]),
                Beard = Convert.ToInt32(data["beard"]),
                BeardOpacity = Convert.ToInt32(data["beardopacity"]),
                BeardColor = Convert.ToInt32(data["beardcolor"])
            };

            if ((bool)data["value"])
            {
                TriggerServerEvent("PlayerCreation", JsonConvert.SerializeObject(playerChar));
                CharOpenned = false;
            }
            else
            {
                SetPedHeadBlendData(ped, playerChar.Dad, playerChar.Mum, playerChar.Dad, playerChar.Skin, playerChar.Skin, playerChar.Skin, (playerChar.DadMumPercent * 0.1f), (float)(playerChar.DadMumPercent * 0.1), 1.0f, true);
                SetPedEyeColor(ped, playerChar.EyeColor);
                SetPedHeadOverlay(ped, 0, playerChar.Acne, (playerChar.Acne == 0) ? 0 : 1);
                SetPedHeadOverlay(ped, 6, playerChar.SkinProblem, 1);
                SetPedHeadOverlay(ped, 9, playerChar.Freckle, (playerChar.Freckle == 0) ? 0 : 1);
                SetPedHeadOverlay(ped, 3, playerChar.Wringle, (playerChar.WringleOpacity * 0.1f));

                SetPedComponentVariation(ped, 2, playerChar.Hair, 0, 2);
                SetPedHairColor(ped, playerChar.HairColor, playerChar.HairColor);

                SetPedHeadOverlay(ped, 2, playerChar.EyeBrow, (playerChar.EyeBrowOpacity * 0.1f));
                SetPedHeadOverlay(ped, 2, playerChar.BeardColor, playerChar.BeardOpacity * 0.1f);
                SetPedHeadOverlayColor(ped, 1, 1, playerChar.BeardColor, playerChar.BeardColor);
                SetPedHeadOverlayColor(ped, 2, 1, playerChar.BeardColor, playerChar.BeardColor);


                // Not Implanted
                SetPedHeadOverlay(GetPlayerPed(-1), 4, 0, 0.0f);  // Lipstick
                SetPedHeadOverlay(GetPlayerPed(-1), 8, 0, 0.0f); // Makeup
                SetPedHeadOverlayColor(GetPlayerPed(-1), 4, 1, 0, 0); // Makeup Color
                SetPedHeadOverlayColor(GetPlayerPed(-1), 8, 1, 0, 0);  // Lipstick Color 
                SetPedComponentVariation(GetPlayerPed(-1), 1, 0, 0, 2); // Mask
            }

            return result;
        }

        // Temp
        private bool firsttick;

        [Tick]
        public Task OnTick()
        {
            if (!firsttick)
            {
                firsttick = true;
                OpenCharCreator();
            }

            if (!CharOpenned)
                return Task.FromResult(0);


            return Task.FromResult(0);
        }

        public void RegisterNUICallback(string msg, Func<IDictionary<string, dynamic>, CallbackDelegate, CallbackDelegate> callback)
        {
            Debug.WriteLine($"Registering NUI EventHandler for {msg}");
            RegisterNuiCallbackType(msg);

            EventHandlers[$"__cfx_nui:{msg}"] += new Action<ExpandoObject, CallbackDelegate>((body, resultCallback) =>
            {
                callback.Invoke(body, resultCallback);
            });
            EventHandlers[$"{msg}"] += new Action<ExpandoObject, CallbackDelegate>((body, resultCallback) =>
            {
                callback.Invoke(body, resultCallback);
            });
        }
    }
}
