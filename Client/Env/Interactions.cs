using CitizenFX.Core;
using CitizenFX.Core.Native;
using UE_Inventory;
using UE_Shared;
using System;
using System.Threading.Tasks;

namespace UE_Client.Env
{
    internal class Interactions : BaseScript
    {
        private static async void InventoryUseItem(dynamic itemID, dynamic Quantity)
        {
            ItemID item = (ItemID)itemID;

            switch (item)
            {
                case ItemID.CampFire:
                    Vector3 pos = Game.Player.Character.Position.Forward(Game.Player.Character.Heading, 3);
                    //await World.CreateProp(Game.GenerateHash("p_campfire02x"), pos, new Vector3(),true, true, true);



                    break;
            }
        }

        private static int tick;

        public Interactions()
        {
            GameMode.RegisterEventHandler("InventoryUseItem", new Action<dynamic, dynamic>(InventoryUseItem));
            Tick += OnTick;
        }

        private static string info = "";
        private static Task OnTick()
        {
            foreach (Control fi in (Control[])Enum.GetValues(typeof(Control)))
            {
                if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, fi))
                {
                    switch (fi)
                    {
                        case Control.Pickup:
                            TriggerServerEvent("OnKeyPress", fi);
                            break;
                    }
                }
            }

            tick++;
            if (tick < 30)
                return Task.FromResult(0);
            tick = 0;

            /*
            Vector3 pos = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_COORD);
            Vector3 dir = Utils.Misc.GetCamDirection();

            Vector3 farAway = pos + dir;
            farAway.Forward(9f, 9f);
            */

            return Task.FromResult(0);
            /*
            RaycastResult? result = Raycast.CastCapsule(pos, farAway, Game.Player.Character.Handle, -1);

            if (result == null)
                return Task.FromResult(0);

            info = $"Hit: {result?.DitHit.ToString()} \n" +
                               $"Entity: {result?.HitEntity.Handle} \n" +
                               $"Coords: X:{result?.HitPosition.X} Y:{result?.HitPosition.Y} Z:{result?.HitPosition.Z}";
            UIHelper.DrawText(info, 0, 0.5f, 0.5f, 0.2f, 0.2f, Color.FromArgb(255, 255, 255, 255), true);

            return Task.FromResult(0);*/
        }
    }
}
