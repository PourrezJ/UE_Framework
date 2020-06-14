using CitizenFX.Core;
using CitizenFX.Core.Native;
using UE_Shared;
using System;
using System.Threading.Tasks;

namespace UE_Client
{
    internal static class RAPI
    {
        internal static void Init()
        {
            Logger.Info("RAPI INIT");
            GameMode.RegisterEventHandler("SetPlayerPosition", new Action<float, float, float, int>(SetPosition));
        }

        private static void SetPosition(float x, float y, float z, int fadedelay = 0)
        {
            if (fadedelay != 0)
            {
                API.DoScreenFadeOut(50);
                Game.Player.Character.Position = new Vector3(x, y, z);
                API.DoScreenFadeIn(50);
            }
            else
                Game.Player.Character.Position = new Vector3(x, y, z);
        }

        internal static async Task CreateVehicle(int hash, Vector3 pos, float head)
        {
            var model = new Model(hash);
            await model.Request(100);

            int spawned = Function.Call<int>(Hash.CREATE_VEHICLE, hash, pos.X, pos.Y, pos.Z, 0, 0, 0, head - 90, true, true, 0, 0);
            Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, spawned, true, true);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, hash);
        }

        internal static Vector3 GetEntityCoords(int entity)
            => Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, entity);

        internal static int GetPedID()
            => Function.Call<int>(Hash.PLAYER_PED_ID); 
    }
}
