using CitizenFX.Core;
using System.Collections.Generic;

namespace UE_Shared.Env
{
    public class TeleportsLocation
    {
        public static Dictionary<string, Location> Teleports = new Dictionary<string, Location>()
        {
            {"Rhodes", new Location(new Vector3(1232.277f,-1277.22f,76.02309f), new Vector3(0,0,319.8914f)) },
            {"Valentine", new Location(new Vector3(-185.4501f,654.6404f,113.397f), new Vector3(0,0,55.10925f)) },
            {"Saint Denis", new Location(new Vector3(2666.083f,-1467.82f,46.31606f), new Vector3(0,0,70.0682f)) },
            {"VanHorm", new Location(new Vector3(2900.87f,637.1765f,56.40763f), new Vector3(0,0,327.6496f)) },
            {"Emerald Station", new Location(new Vector3(1515.86f,429.2264f,89.82934f), new Vector3(0,0,100.4891f)) },
            {"StrawBerry", new Location(new Vector3(-1835.54f,-441.6235f,159.7826f), new Vector3(0,0,346.4001f)) },
            {"Armadillo", new Location(new Vector3(-3726.746f,-2625.305f,-13.28079f), new Vector3(0,0,262.514f)) },
            {"Tumbleweed", new Location(new Vector3(-5427.289f,-2944.223f,0.9996545f), new Vector3(0,0,354.3692f)) },
            {"BlackWater", new Location(new Vector3(-855.1125f,-1345.339f,43.4208f), new Vector3(0,0,167.3936f)) },
            {"Macfarlane Ranch", new Location(new Vector3(-2502.046f,-2448.097f,60.17968f), new Vector3(0,0,204.6972f)) },
        };
    }
}
