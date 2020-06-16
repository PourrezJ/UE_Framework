using CitizenFX.Core.Native;

namespace UE_Client
{
    public static class IplLoader
    {
        private static int[] iplList = new int[]
        {

        };

        public static void Init()
        {
            for (int i = 0; i < iplList.Length; i++)
            {
                if (!Function.Call<bool>(Hash.IS_IPL_ACTIVE, iplList[i]))
                {
                    Function.Call(Hash.REQUEST_IPL, iplList[i]);
                }
            }

        }
    }
}
