using CitizenFX.Core;

namespace UE_Shared
{
    public class Location
    {
        public Vector3 Pos;
        public Vector3 Rot;

        public Location(Vector3 pos, Vector3 rot)
        {
            Pos = pos;
            Rot = rot;
        }
    }
}
