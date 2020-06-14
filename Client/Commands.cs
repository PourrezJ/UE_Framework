using CitizenFX.Core;
using CitizenFX.Core.Native;
using UE_Shared;
using System;
using System.Collections.Generic;

namespace UE_Client
{
    internal static class Commands
    {
        internal static void Init()
        {
            /*
            API.RegisterCommand("veh", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var pos = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, API.GetPlayerPed(0));

                pos = pos.Forward(0, 5);
   
                await RAPI.CreateVehicle(RDResurrection_Shared.Utils.GetHashKey(args[0].ToString()), pos, 0);
            }), false);*/

            API.RegisterCommand("tpto", new Action<int, List<object>, string>((source, args, raw) =>
            {
                float x = Convert.ToSingle(args[0].ToString().Replace(',',' '));
                float y = Convert.ToSingle(args[1].ToString().Replace(',', ' '));
                float z = Convert.ToSingle(args[2].ToString().Replace(',', ' '));

                Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, RAPI.GetPedID(), x, y, z);
            }), false);

            API.RegisterCommand("cloth", new Action<int, List<object>, string>((source, args, raw) =>
            {
                Function.Call((Hash)0x283978A15512B2FE, RAPI.GetPedID(), true);
                Function.Call((Hash)0xA5BAE410B03E7371, RAPI.GetPedID(), Convert.ToInt32(args[0]), 0, 0, 0);
            }), false);

            API.RegisterCommand("clothtest", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                for(int i = 0; i < (Game.Player.Character.Model == Game.GenerateHash("mp_male") ?  109 : 95); i++)
                {
                    Logger.Info(i.ToString());
                    Function.Call((Hash)0x283978A15512B2FE, RAPI.GetPedID(), true);
                    Function.Call((Hash)0xA5BAE410B03E7371, RAPI.GetPedID(), i, 0, 0, 0);

                    await GameMode.Delay(1500);
                }
            }), false);

            API.RegisterCommand("coords", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count == 0)
                {
                    Logger.Warn("Utilisation : coords [nom]");
                    return;
                }

                string coordName = Convert.ToString(args[0]);

                Vector3 playerPosGet = Game.Player.Character.Position;
                float playerRotGet = Game.Player.Character.Heading;

                string pPosX = (playerPosGet.X.ToString().Replace(',', '.'));
                string pPosY = (playerPosGet.Y.ToString().Replace(',', '.'));
                string pPosZ = (playerPosGet.Z.ToString().Replace(',', '.'));

                GameMode.TriggerServerEvent("RegisterCoords", coordName, pPosX, pPosY, pPosZ, playerRotGet.ToString());
                Logger.Info("Your position is: ~y~" + playerPosGet + "~w~Your rotation is: ~y~" + playerRotGet);
            }), false);

            API.RegisterCommand("apply", new Action<int, List<object>, string>((source, args, raw) =>
            {
                int hash = Convert.ToInt32(args[0]);
                /*
                var category = API.N_0x5ff9a878c3d115b8(hash, 0, 1);
                API.N_0x59bd177a1a48600a(Game.Player.Character.Handle, category);
                API.N_0xd3a7b003ed343fd9(Game.Player.Character.Handle, hash, 1, 0, 0);
                API.N_0xd3a7b003ed343fd9(Game.Player.Character.Handle, hash, 1, 1, 0);*/

                Function.Call((Hash)0x283978A15512B2FE, API.PlayerPedId(), true);
                Function.Call((Hash)0xA5BAE410B03E7371, API.PlayerPedId(), hash, 0, 0, 0);
            }), false);

            API.RegisterCommand("toast", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                await GameMode.Delay(2000);
                string test = "Bonjour Resurrection \n" +
                "Petit test de notre système de notification\n" +
                "~r~It's Fine!";

                Toast.AddToast(test, 15000, 0.88f, 0.2f);
            }), false);
        }
    }
}
