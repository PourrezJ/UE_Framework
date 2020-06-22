using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Text;
using System.Threading.Tasks;

namespace UE_Client_Extented
{
    public class Utils
    {
        public static async Task PerformRequest(PedHash hash) => await PerformRequest((int)hash);

        public static async Task PerformRequest(int hash)
        {
            if (Function.Call<bool>(Hash.IS_MODEL_IN_CDIMAGE, hash) && Function.Call<bool>(Hash.IS_MODEL_VALID, hash))
            {
                Debug.WriteLine($"Requesting model {hash}");

                Function.Call(Hash.REQUEST_MODEL, hash);

                while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, hash))
                {
                    await BaseScript.Delay(50);
                }
            }
            else
            {
                Debug.WriteLine("Requested model not valid");
            }
        }

        public static int GetHashKey(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text.ToLowerInvariant());

            uint num = 0u;

            for (int i = 0; i < bytes.Length; i++)
            {
                num += (uint)bytes[i];
                num += num << 10;
                num ^= num >> 6;
            }
            num += num << 3;
            num ^= num >> 11;

            return (int)(num + (num << 15));
        }

        public static bool LoadScript(string script)
        {
            if (!Function.Call<bool>(Hash.DOES_SCRIPT_EXIST, script))
                return false;

            Function.Call(Hash.REQUEST_SCRIPT, script);
            if (!Function.Call<bool>(Hash.HAS_SCRIPT_LOADED, script))
                return false;

            var scriptID = API.StartNewScript(script, 1024);
            Function.Call(Hash.SET_SCRIPT_WITH_NAME_HASH_AS_NO_LONGER_NEEDED, script);
            return true;
        }
    }
}
