using System;

namespace DeviceHandler
{
    public static class ToBigEndian
    {
        public static int Convert(int num)
        {
            return (int)((num & 0x000000FF) << 24) |
                (int)((num & 0x0000FF00) << 8) |
                (int)((num & 0x00FF0000) >> 8) |
                (int)((num & 0xFF000000) >> 24);
        }
    }
}
