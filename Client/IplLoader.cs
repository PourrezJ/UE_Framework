using CitizenFX.Core.Native;

namespace UE_Client
{
    public static class IplLoader
    {
        private static int[] iplList = new int[]
        {
            -37875204,
            258104717,
            -76700394,
            1614255891,
            1861460906,
            1044079550,
            -1276109918,
            -1386423483,
            -1405375965,
            1277540472,
            -1331593143,
            1133172356,
            -559257162,
            -574996782,
            1169511062,
            -1266106154,
            -1377975054,
            897624424,
            -2111718052,
            175578406,
            -686953321,
            -1737485501,
            -739754595,
            942470447,
            -1859413313,
            489834626,
            -5339556,
            1258244391,
            1343343014,
            -2082201137,
            1641449717,
            739412171,
            -501793326,
            466168676,
            903666582,
            -1509154451,
            158063004,
            -1112373128,
            -891994084,
            -84516711,
            -657241692,
            1149195254,
            2016081133,
            363257921,
            58066174,
            -1521525254,
            -761186147,
            -1872939092,
            -964156415,
                };

        public static void Init()
        {


            /*
            if (!STREAMING::_IS_IMAP_ACTIVE(iParam0))
            {
                func_119(iParam0);
            }*/


           // Function.Call(Hash._REQUEST_IMAP, -892659042);
           // Function.Call(Hash._REQUEST_IMAP, 1777348822);


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
