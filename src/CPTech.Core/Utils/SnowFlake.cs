using Snowflake.Core;

namespace CPTech.Core
{
    public class SnowFlake
    {
        private static readonly IdWorker worker = new IdWorker(1, 1);

        public static long NextId()
        {
            return worker.NextId();
        }
    }
}
