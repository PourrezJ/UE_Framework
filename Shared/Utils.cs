using System;
using System.Text;

namespace UE_Shared
{
    public static class Utils
    {
        public static int RandomNumber(int max) => new Random().Next(max);
        public static int RandomNumber(int min, int max) => new Random().Next(min, max);

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
    }
}
