using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Utils
{
    public class UtilTime
    {
        public static long GetUnixTime()
        {
            var baseDt = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var unixtime = (DateTimeOffset.Now - baseDt).Ticks / 10;//usec
            return unixtime;
        }

        public static bool IsTimeout(long start_time, long timeout)
        {
            long current_time = GetUnixTime();
            if (current_time >= (start_time + timeout))
            {
                return true;
            }
            return false;
        }
    }
}
