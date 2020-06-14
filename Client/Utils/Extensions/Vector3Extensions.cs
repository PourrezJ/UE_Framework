using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Client.Utils.Extensions
{
    internal static class Vector3Extensions
    {
        public static bool IsInArea(this Vector3 pos, Vector3[] pORect)
        {
            float distance = (pORect[0].DistanceToSquared2D(pORect[2]) < pORect[1].DistanceToSquared2D(pORect[3]) ? pORect[0].DistanceToSquared2D(pORect[2]) : pORect[1].DistanceToSquared2D(pORect[2])) / 1.7f;

            int count = 0;
            foreach (Vector3 v in pORect)
                if (pos.DistanceToSquared2D(v) <= distance) count++;

            return count >= 2;
        }

        public static Vector3 Backward(this Vector3 point, float rot, float dist)
        {
            var angle = rot;
            double xOff = (Math.Cos((angle * Math.PI) / 180) * dist);
            double yOff = -(Math.Sin((angle * Math.PI) / 180) * dist);

            return point + new Vector3((float)xOff, (float)yOff, 0);
        }

        public static float ClampAngle(float angle)
        {
            return (float)(angle + Math.Ceiling(-angle / 360) * 360);
        }
    }
}