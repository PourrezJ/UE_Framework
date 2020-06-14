using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Client.Helpers
{
    internal static class Discord
    {
        internal static string ID;

        internal static void Init(string id)
        {
            ID = id;

            API.SetDiscordAppId(id);
            API.SetDiscordRichPresenceAssetSmall("red-dead-redemption-2");
            API.SetDiscordRichPresenceAssetSmallText("test");
        }
    }
}
