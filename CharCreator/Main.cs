using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharCreator
{
    internal class Main : BaseScript
    {
        internal Main()
        {
        }

        [EventHandler("OpenCharCreator")]
        internal void OpenCharCreator()
        {
            API.SetNuiFocus(true, true);
            JsonConvert.SerializeObject(new { openSkinCreator = true });
        }

        [Tick]
        private void OnTick()
        {

        }
    }
}
